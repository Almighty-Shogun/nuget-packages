using System.Net;
using System.Text;
using System.Text.Json;
using System.Net.Sockets;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Accepts and dispatches remote command requests.
/// </summary>
///
/// <param name="remoteServerSettings">
/// The remote server settings.
/// </param>
/// <param name="logger">The logger used by the handler.</param>
/// <param name="descriptors">
/// The registered remote command descriptors.
/// </param>
/// <param name="scopeFactory">
/// The service scope factory used to resolve commands.
/// </param>
///
/// <exception cref="InvalidOperationException">
/// The configured address or whitelist is invalid, or a registered command type is not dispatchable.
/// </exception>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
internal sealed class RemoteCommandHandler(
    IOptions<RemoteServerSettings> remoteServerSettings,
    ILogger<RemoteCommandHandler> logger,
    IEnumerable<RemoteCommandDescriptor> descriptors,
    IServiceScopeFactory scopeFactory
) : IRemoteCommandHandler
{
    private readonly RemoteServerSettings _config = remoteServerSettings.Value;

    private readonly IReadOnlyList<IPNetwork> _whitelist =
        RemoteServerSettingsParser.ParseWhitelist(remoteServerSettings.Value.Whitelisted);

    private readonly IPAddress _address = RemoteServerSettingsParser.ParseAddress(remoteServerSettings.Value.Address);

    /// <summary>
    /// The digest of the configured pre-shared key, or <c>null</c> when no key is required.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private readonly byte[]? _secret = remoteServerSettings.Value.Secret is { Length: > 0 } secret
        ? SHA256.HashData(Encoding.UTF8.GetBytes(secret))
        : null;

    private readonly Dictionary<string, RemoteCommandDescriptor> _commands = BuildCommandTable(descriptors, logger);
    private readonly SemaphoreSlim _connectionLimit = new(remoteServerSettings.Value.MaxConcurrentConnections);
    private readonly Lock _lifecycleGate = new();
    private readonly List<Task> _inFlight = [];
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
                try
                {
                    await _connectionLimit.WaitAsync(stopSource.Token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }

                var transferred = false;

                try
                {
                    TcpClient client = await listener.AcceptTcpClientAsync(stopSource.Token);

                    Task handling = HandleClientSafelyAsync(client, stopSource.Token);

                    transferred = true;

                    lock (_lifecycleGate)
                    {
                        _inFlight.RemoveAll(task => task.IsCompleted);
                        _inFlight.Add(handling);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                finally
                {
                    if (!transferred)
                        _connectionLimit.Release();
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
    /// Builds the command dispatch table.
    /// </summary>
    ///
    /// <param name="descriptors">The registered command descriptors.</param>
    /// <param name="logger">The logger used to report duplicate command names.</param>
    ///
    /// <returns>
    /// The command descriptors keyed by command name.
    /// </returns>
    ///
    /// <remarks>
    /// When multiple commands use the same name, the first registration is retained.
    /// </remarks>
    /// 
    /// <exception cref="InvalidOperationException">
    /// A registered command type does not implement the internal dispatch contract.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static Dictionary<string, RemoteCommandDescriptor> BuildCommandTable(
        IEnumerable<RemoteCommandDescriptor> descriptors,
        ILogger logger
    )
    {
        Dictionary<string, RemoteCommandDescriptor> table = new(StringComparer.Ordinal);

        foreach (RemoteCommandDescriptor descriptor in descriptors)
        {
            if (!typeof(IInternalRemoteCommand).IsAssignableFrom(descriptor.ImplementationType))
                throw new InvalidOperationException($"{descriptor.ImplementationType.Name} must inherit {nameof(RemoteCommand<>)}.");

            if (!table.TryAdd(descriptor.Name, descriptor) && logger.IsEnabled(LogLevel.Warning))
                logger.LogWarning(
                    "{Name:y} is already registered, so {Skipped:c} will never be dispatched",
                    descriptor.Name,
                    descriptor.ImplementationType.Name);
        }

        return table;
    }

    /// <summary>
    /// Handles an accepted connection and releases its connection slot when finished.
    /// </summary>
    ///
    /// <param name="client">The accepted client.</param>
    /// <param name="cancellationToken">A token used to stop connection handling.</param>
    ///
    /// <returns>
    /// A task that completes when connection handling finishes.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
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
    /// Serves requests from an accepted client until the connection ends.
    /// </summary>
    ///
    /// <param name="client">The accepted client.</param>
    /// <param name="cancellationToken">A token used to stop connection handling.</param>
    ///
    /// <returns>
    /// A task that completes when the connection is no longer being served.
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
                    string.Join(", ", _config.Whitelisted));

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
    /// Dispatches a request frame and writes its response.
    /// </summary>
    ///
    /// <param name="frame">The request payload.</param>
    /// <param name="stream">The stream used to write the response.</param>
    /// <param name="remoteEndPoint">The remote client endpoint.</param>
    /// <param name="cancellationToken">A token used to cancel request processing.</param>
    ///
    /// <returns>
    /// A task that completes when request processing finishes.
    /// </returns>
    ///
    /// <remarks>
    /// Malformed requests, missing command names, failed authentication, unknown commands, and invalid command messages
    /// are returned as their corresponding <see cref="RemoteCommandRefusal"/> values. Command failures are returned as
    /// <see cref="RemoteCommandRefusal.Other"/> when the command has not already written a response. A successful command
    /// that writes no response receives an empty acknowledgement.
    /// </remarks>
    /// 
    /// <exception cref="OperationCanceledException">
    /// Request processing was canceled.
    /// </exception>
    /// <exception cref="IOException">
    /// A response could not be written to the connection.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
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
                cancellationToken);

            return;
        }

        if (payload is null || string.IsNullOrWhiteSpace(payload.Command))
        {
            if (logger.IsEnabled(LogLevel.Warning))
                logger.LogWarning("Received a payload from {Address:c} with no command name", remoteEndPoint);

            await RemoteCommandProtocol.WriteFrameAsync(
                stream,
                RemoteCommandResponse.Refused(RemoteCommandRefusal.MissingCommandName),
                cancellationToken);

            return;
        }

        if (!IsSecretValid(payload.Secret))
        {
            if (logger.IsEnabled(LogLevel.Warning))
                logger.LogWarning(
                    "Rejected {Command:y} from {Address:c} because the pre-shared key did not match",
                    payload.Command,
                    remoteEndPoint);

            await RemoteCommandProtocol.WriteFrameAsync(
                stream,
                RemoteCommandResponse.Refused(RemoteCommandRefusal.Unauthorized),
                cancellationToken);

            return;
        }

        if (!_commands.TryGetValue(payload.Command, out RemoteCommandDescriptor? descriptor))
        {
            if (logger.IsEnabled(LogLevel.Warning))
                logger.LogWarning("Received unknown remote command {Command:y} from {Address:c}", payload.Command, remoteEndPoint);

            await RemoteCommandProtocol.WriteFrameAsync(
                stream,
                RemoteCommandResponse.Refused(RemoteCommandRefusal.CommandNotFound),
                cancellationToken);

            return;
        }

        if (logger.IsEnabled(LogLevel.Information) && _config.EnableReceiveLog)
            logger.LogInformation("Received remote command {Command:y} from {Address:c}", payload.Command, remoteEndPoint);

        StreamCommandResponse response = new(stream);

        await using AsyncServiceScope scope = scopeFactory.CreateAsyncScope();

        var command = (IInternalRemoteCommand)scope.ServiceProvider.GetRequiredService(descriptor.ImplementationType);

        object message;

        try
        {
            message = command.Bind(payload.Data);
        }
        catch (JsonException exception)
        {
            if (logger.IsEnabled(LogLevel.Warning))
                logger.LogWarning("The {Command:y} payload did not match its message type: {Reason:c}", payload.Command, exception.Message);

            await RemoteCommandProtocol.WriteFrameAsync(
                stream,
                RemoteCommandResponse.Refused(RemoteCommandRefusal.InvalidMessage),
                cancellationToken);

            return;
        }

        try
        {
            await command.ExecuteAsync(message, response, cancellationToken);
        }
        catch (Exception exception) when (exception is not OperationCanceledException || !cancellationToken.IsCancellationRequested)
        {
            logger.LogError(exception, "The {Command:y} remote command failed", payload.Command);

            if (!response.HasWritten)
                await RemoteCommandProtocol.WriteFrameAsync(
                    stream,
                    RemoteCommandResponse.Refused(RemoteCommandRefusal.Other),
                    cancellationToken);

            return;
        }

        if (!response.HasWritten)
            await RemoteCommandProtocol.WriteFrameAsync(stream, new RemoteCommandResponse(), cancellationToken);
    }

    /// <summary>
    /// Checks whether a remote address is allowed by the configured whitelist.
    /// </summary>
    ///
    /// <param name="address">
    /// The remote address.
    /// </param>
    ///
    /// <returns><c>true</c> if the address is allowed; otherwise, <c>false</c>.</returns>
    ///
    /// <remarks>
    /// IPv4-mapped IPv6 addresses are normalized to IPv4 before matching.
    /// </remarks>
    /// 
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private bool IsWhitelisted(IPAddress? address)
    {
        if (address is null)
            return false;

        IPAddress candidate = address.IsIPv4MappedToIPv6 ? address.MapToIPv4() : address;

        return _whitelist.Any(network => network.Contains(candidate));
    }

    /// <summary>
    /// Checks whether the supplied pre-shared key is valid.
    /// </summary>
    ///
    /// <param name="supplied">The supplied pre-shared key.</param>
    ///
    /// <returns>
    /// <c>true</c> if no key is required or the supplied key matches; otherwise, <c>false</c>.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private bool IsSecretValid(string? supplied)
    {
        if (_secret is null)
            return true;

        return supplied is not null
               && CryptographicOperations.FixedTimeEquals(SHA256.HashData(Encoding.UTF8.GetBytes(supplied)), _secret);
    }
}
