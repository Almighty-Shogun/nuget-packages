using System.Net;
using System.Text;
using System.Text.Json;
using System.Net.Sockets;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Accepts connections on the configured endpoint and serves framed requests on each until the client goes away. A
/// connection outlives one request, so a caller may run several commands without reconnecting.
/// </summary>
///
/// <param name="remoteServerSettings">
/// The bound listener settings. Read once into fields, so a reload does not change the behavior of a listener that is
/// already bound, and the address and whitelist are parsed here rather than on every connection.
/// </param>
/// <param name="logger">The logger every lifecycle event and rejected request is reported through.</param>
/// <param name="commands">
/// Every registered command, enumerated once to build the dispatch table. This is where a malformed command surfaces,
/// because constructing it runs the validation in <see cref="RemoteCommand{T}"/>.
/// </param>
///
/// <exception cref="InvalidOperationException">
/// Thrown while constructing, when the configured address or a whitelist entry does not parse, or a registered command
/// does not derive from <see cref="RemoteCommand{T}"/>. Everything the listener needs is therefore settled before it can
/// be started, rather than failing part-way through binding.
/// </exception>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
internal sealed class RemoteCommandHandler(
    IOptions<RemoteServerSettings> remoteServerSettings,
    ILogger<RemoteCommandHandler> logger,
    IEnumerable<IRemoteCommand> commands
) : IRemoteCommandHandler
{
    /// <summary>
    /// The settings snapshot the listener runs on, taken at construction rather than read per request.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>3.0.0</since>
    private readonly RemoteServerSettings _config = remoteServerSettings.Value;

    /// <summary>
    /// The whitelist as networks rather than strings, parsed once. An empty list matches nothing, so a configuration
    /// without a whitelist refuses every connection rather than accepting every one.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly IReadOnlyList<IPNetwork> _whitelist =
        RemoteServerSettingsParser.ParseWhitelist(remoteServerSettings.Value.Whitelisted);

    /// <summary>
    /// The address the listener binds to, parsed at construction so a bad value is reported when the handler is resolved
    /// rather than when it first tries to bind.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly IPAddress _address = RemoteServerSettingsParser.ParseAddress(remoteServerSettings.Value.Address);

    /// <summary>
    /// The required key as bytes, or <c>null</c> when the server asks for none. Held encoded so each comparison is a
    /// fixed-time byte compare rather than a string equality that returns early on the first wrong character.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly byte[]? _secret = remoteServerSettings.Value.Secret is { Length: > 0 } secret
        ? Encoding.UTF8.GetBytes(secret)
        : null;

    /// <summary>
    /// Every command name mapped to its entry point, compared with ordinal case sensitivity so a wire name must match
    /// exactly. Built once, because the set of commands cannot change while the process runs.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>3.0.0</since>
    private readonly Dictionary<string, IInternalRemoteCommand> _commands = BuildCommandTable(commands, logger);

    /// <summary>
    /// The cap on connections served at once. A further client is accepted only when a slot frees, so load is refused by
    /// waiting rather than by exhausting the process.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly SemaphoreSlim _connectionLimit = new(remoteServerSettings.Value.MaxConcurrentConnections);

    /// <summary>
    /// Guards the lifecycle source and the in-flight list, so a stop cannot observe either mid-update.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly Lock _lifecycleGate = new();

    /// <summary>
    /// The connections still being served, awaited for five seconds on shutdown so a client receiving a response is not
    /// cut off mid-frame. Completed entries are pruned as new ones are added.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly List<Task> _inFlight = [];

    /// <summary>
    /// The source cancelled to stop the listener, and the flag for whether one is running: non-null only between the
    /// start of a listener and its exit.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private CancellationTokenSource? _stopSource;

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        CancellationTokenSource stopSource;

        lock (_lifecycleGate)
        {
            if (_stopSource is not null)
            {
                logger.LogError("Cannot start the remote command handler because it is already running.");

                return;
            }

            stopSource = _stopSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        }

        TcpListener listener = new(_address, _config.Port);

        try
        {
            listener.Start();

            if (logger.IsEnabled(LogLevel.Information))
                logger.LogInformation("Started listening for remote commands on {Address:c}:{Port:c}", _config.Address, _config.Port);

            while (!stopSource.IsCancellationRequested)
            {
                TcpClient client;

                try
                {
                    await _connectionLimit.WaitAsync(stopSource.Token);

                    client = await listener.AcceptTcpClientAsync(stopSource.Token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }

                Task handling = HandleClientSafelyAsync(client, stopSource.Token);

                lock (_lifecycleGate)
                {
                    _inFlight.RemoveAll(task => task.IsCompleted);
                    _inFlight.Add(handling);
                }
            }
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "The remote command handler stopped unexpectedly.");
        }
        finally
        {
            listener.Stop();

            Task[] pending;

            lock (_lifecycleGate)
            {
                pending = [.. _inFlight];
            }

            await Task.WhenAny(Task.WhenAll(pending), Task.Delay(TimeSpan.FromSeconds(5), CancellationToken.None));

            lock (_lifecycleGate)
            {
                _stopSource?.Dispose();
                _stopSource = null;
            }
        }
    }

    /// <inheritdoc />
    public void Stop()
    {
        lock (_lifecycleGate)
        {
            if (_stopSource is null)
            {
                logger.LogError("Cannot stop the remote command handler because it is not running.");

                return;
            }

            _stopSource.Cancel();
        }

        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation("Stopped listening for remote commands.");
    }

    /// <summary>
    /// Builds the dispatch table once, keeping the first command to claim each name rather than failing the whole
    /// listener over a collision.
    /// </summary>
    ///
    /// <param name="commands">The registered commands, each resolved once here.</param>
    /// <param name="logger">The logger a dropped duplicate name is reported through.</param>
    ///
    /// <returns>
    /// Every usable command keyed by name. A name claimed twice keeps its first command, so registration order decides
    /// the winner and the loser is unreachable.
    /// </returns>
    ///
    /// <exception cref="InvalidOperationException">
    /// A registered command does not derive from <see cref="RemoteCommand{T}"/> and so exposes no entry point to
    /// dispatch to.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static Dictionary<string, IInternalRemoteCommand> BuildCommandTable(IEnumerable<IRemoteCommand> commands, ILogger logger)
    {
        Dictionary<string, IInternalRemoteCommand> table = new(StringComparer.Ordinal);

        foreach (IRemoteCommand command in commands)
        {
            if (command is not IInternalRemoteCommand internalCommand)
                throw new InvalidOperationException($"{command.GetType().Name} must inherit {nameof(RemoteCommand<>)}.");

            if (!table.TryAdd(command.Name, internalCommand) && logger.IsEnabled(LogLevel.Warning))
                logger.LogWarning(
                    "{Name:y} is already registered, so {Skipped:c} will never be dispatched",
                    command.Name,
                    command.GetType().Name
                );
        }

        return table;
    }

    /// <summary>
    /// Serves one connection and swallows whatever escapes, because nothing awaits the task this returns and an escaping
    /// failure would surface as an unobserved exception instead of a log line.
    /// </summary>
    ///
    /// <param name="client">The accepted connection, disposed by the inner call whatever happens.</param>
    /// <param name="cancellationToken">Signaled when the listener is stopping.</param>
    ///
    /// <returns>
    /// A task that always completes successfully. Nothing awaits it for a result, so a failure allowed to escape would
    /// surface as an unobserved exception rather than a log line.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task HandleClientSafelyAsync(TcpClient client, CancellationToken cancellationToken)
    {
        try
        {
            await HandleClientAsync(client, cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Failed to handle a remote command client.");
        }
        finally
        {
            _connectionLimit.Release();
        }
    }

    /// <summary>
    /// Checks the client against the whitelist, then serves its requests one at a time until it disconnects, goes idle
    /// past the timeout, or sends something unreadable.
    /// </summary>
    ///
    /// <param name="client">The accepted connection, taken over and disposed when this returns.</param>
    /// <param name="cancellationToken">Signaled when the listener is stopping.</param>
    ///
    /// <returns>
    /// A task that completes when the client disconnects, goes idle past the timeout, or sends something unreadable. The
    /// connection is kept open between requests, so one client may run many commands on it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private async Task HandleClientAsync(TcpClient client, CancellationToken cancellationToken)
    {
        using TcpClient owned = client;

        var remoteEndPoint = owned.Client.RemoteEndPoint as IPEndPoint;

        if (!IsWhitelisted(remoteEndPoint?.Address))
        {
            if (logger.IsEnabled(LogLevel.Warning))
                logger.LogWarning(
                    "Rejected connection from {Address:c}; whitelist is {Whitelist:c}",
                    remoteEndPoint?.Address,
                    string.Join(", ", _config.Whitelisted)
                );

            return;
        }

        await using NetworkStream stream = owned.GetStream();

        while (!cancellationToken.IsCancellationRequested)
        {
            using var idleSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            idleSource.CancelAfter(TimeSpan.FromSeconds(_config.IdleTimeout));

            byte[]? frame;

            try
            {
                frame = await RemoteCommandProtocol.ReadFrameAsync(stream, _config.MaxPayloadBytes, idleSource.Token);
            }
            catch (OperationCanceledException)
            {
                return;
            }
            catch (Exception exception) when (exception is EndOfStreamException or InvalidDataException or IOException)
            {
                if (logger.IsEnabled(LogLevel.Warning))
                    logger.LogWarning("Discarded a malformed frame from {Address:c}: {Reason:c}", remoteEndPoint, exception.Message);

                return;
            }

            if (frame is null) return;

            using var readSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            readSource.CancelAfter(TimeSpan.FromSeconds(_config.ReadTimeout));

            await DispatchAsync(frame, stream, remoteEndPoint, readSource.Token);
        }
    }

    /// <summary>
    /// Turns one request frame into a command invocation, answering with a refusal rather than a dropped connection
    /// whenever it cannot be served.
    /// </summary>
    ///
    /// <param name="frame">The request bytes, one complete frame.</param>
    /// <param name="stream">The connection to answer on.</param>
    /// <param name="remoteEndPoint">The client address, used only to name it in the log.</param>
    /// <param name="cancellationToken">Signaled when the read timeout elapses or the listener is stopping.</param>
    ///
    /// <returns>
    /// A task that completes once exactly one envelope has been sent, whether it carries the command's own response, a
    /// refusal, or neither, which is how a command that returned without answering is acknowledged.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task DispatchAsync(byte[] frame, Stream stream, IPEndPoint? remoteEndPoint, CancellationToken cancellationToken)
    {
        RemoteCommandPayload? payload;

        try
        {
            payload = JsonSerializer.Deserialize<RemoteCommandPayload>(frame, RemoteCommandProtocol.SerializerOptions);
        }
        catch (JsonException exception)
        {
            if (logger.IsEnabled(LogLevel.Warning))
                logger.LogWarning("Received malformed JSON from {Address:c}: {Reason:c}", remoteEndPoint, exception.Message);

            await RemoteCommandProtocol.WriteFrameAsync(
                stream,
                RemoteCommandResponse.Refused(RemoteCommandRefusal.MalformedPayload),
                cancellationToken
            );

            return;
        }

        if (payload is null || string.IsNullOrWhiteSpace(payload.Command))
        {
            if (logger.IsEnabled(LogLevel.Warning))
                logger.LogWarning("Received a payload from {Address:c} with no command name", remoteEndPoint);

            await RemoteCommandProtocol.WriteFrameAsync(
                stream,
                RemoteCommandResponse.Refused(RemoteCommandRefusal.MissingCommandName),
                cancellationToken
            );

            return;
        }

        if (!IsSecretValid(payload.Secret))
        {
            if (logger.IsEnabled(LogLevel.Warning))
                logger.LogWarning(
                    "Rejected {Command:y} from {Address:c} because the pre-shared key did not match",
                    payload.Command,
                    remoteEndPoint
                );

            await RemoteCommandProtocol.WriteFrameAsync(
                stream,
                RemoteCommandResponse.Refused(RemoteCommandRefusal.Unauthorized),
                cancellationToken
            );

            return;
        }

        if (!_commands.TryGetValue(payload.Command, out IInternalRemoteCommand? command))
        {
            if (logger.IsEnabled(LogLevel.Warning))
                logger.LogWarning("Received unknown remote command {Command:y} from {Address:c}", payload.Command, remoteEndPoint);

            await RemoteCommandProtocol.WriteFrameAsync(
                stream,
                RemoteCommandResponse.Refused(RemoteCommandRefusal.CommandNotFound),
                cancellationToken
            );

            return;
        }

        if (logger.IsEnabled(LogLevel.Information) && _config.EnableReceiveLog)
            logger.LogInformation("Received remote command {Command:y} from {Address:c}", payload.Command, remoteEndPoint);

        StreamCommandResponse response = new(stream);

        try
        {
            await command.HandleRawAsync(payload.Data, response, cancellationToken);
        }
        catch (JsonException exception)
        {
            if (logger.IsEnabled(LogLevel.Warning))
                logger.LogWarning("The {Command:y} payload did not match its message type: {Reason:c}", payload.Command, exception.Message);

            if (!response.HasWritten)
                await RemoteCommandProtocol.WriteFrameAsync(
                    stream,
                    RemoteCommandResponse.Refused(RemoteCommandRefusal.InvalidMessage),
                    cancellationToken
                );

            return;
        }

        if (!response.HasWritten)
            await RemoteCommandProtocol.WriteFrameAsync(stream, new RemoteCommandResponse(), cancellationToken);
    }

    /// <summary>
    /// Checks a remote address against the parsed whitelist.
    /// </summary>
    ///
    /// <param name="address">
    /// The connecting address, or <c>null</c> when the socket reported no endpoint. An IPv4-mapped IPv6 address is
    /// unmapped first, so a rule written as IPv4 still matches a client arriving on a dual-stack socket.
    /// </param>
    ///
    /// <returns><c>true</c> when some configured network contains the address; otherwise <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private bool IsWhitelisted(IPAddress? address)
    {
        if (address is null)
            return false;

        IPAddress candidate = address.IsIPv4MappedToIPv6 ? address.MapToIPv4() : address;

        return _whitelist.Any(network => network.Contains(candidate));
    }

    /// <summary>
    /// Compares the supplied pre-shared key in constant time.
    /// </summary>
    ///
    /// <param name="supplied">The key from the request, or <c>null</c> when the client sent none.</param>
    ///
    /// <returns>
    /// <c>true</c> when the server requires no key, or when the supplied one matches. Comparison takes the same time
    /// whatever the value, so a wrong key cannot be recovered a character at a time.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private bool IsSecretValid(string? supplied)
    {
        if (_secret is null)
            return true;

        return supplied is not null && CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(supplied), _secret);
    }
}
