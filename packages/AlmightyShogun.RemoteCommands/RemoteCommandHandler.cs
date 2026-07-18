using System.Net;
using System.Text;
using System.Text.Json;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Listens for TCP remote command payloads and dispatches them to registered command handlers.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
internal class RemoteCommandHandler : IRemoteCommandHandler
{
    /// <summary>
    /// Stores the active TCP listener while the handler is running.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private TcpListener? _listener;

    /// <summary>
    /// Stores the bound listener configuration.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>3.0.0</since>
    private readonly RemoteServerSettings _config;

    /// <summary>
    /// Stores the logger used for listener lifecycle, validation, and dispatch messages.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private readonly ILogger<RemoteCommandHandler> _logger;

    /// <summary>
    /// Stores command names mapped to their internal raw-payload handlers.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>3.0.0</since>
    private readonly Dictionary<string, IInternalRemoteCommand> _commands;

    /// <summary>
    /// Creates a remote command handler from configuration and registered commands.
    /// </summary>
    ///
    /// <param name="remoteServerSettings">The bound remote server settings used by the listener.</param>
    /// <param name="logger">The logger used to report lifecycle and command dispatch events.</param>
    /// <param name="commands">The command services registered in dependency injection.</param>
    ///
    /// <exception cref="InvalidOperationException">Thrown when a registered command does not inherit from <see cref="RemoteCommand{T}"/>.</exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>3.0.0</since>
    public RemoteCommandHandler(IOptions<RemoteServerSettings> remoteServerSettings, ILogger<RemoteCommandHandler> logger, IEnumerable<IRemoteCommand> commands)
    {
        _logger = logger;
        _config = remoteServerSettings.Value;
        _commands = new Dictionary<string, IInternalRemoteCommand>();

        foreach (IRemoteCommand command in commands)
        {
            if (command is not IInternalRemoteCommand internalCommand)
            {
                throw new InvalidOperationException($"{command.GetType().Name} must inherit {nameof(RemoteCommand<>)}.");
            }

            _commands.Add(command.Name, internalCommand);
        }
    }

    /// <inheritdoc />
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.2.0</since>
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (_listener is not null)
        {
            _logger.LogError("Cannot start the remote command handler because it is already running.");

            return;
        }

        TcpListener? listener = null;

        try
        {
            listener = new TcpListener(IPAddress.Parse(_config.Address), _config.Port);
            _listener = listener;

            listener.Start();

            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Started listening for remote command on {Address:c}:{Port:c}", _config.Address, _config.Port);
            }

            while (!cancellationToken.IsCancellationRequested)
            {
                TcpClient client;

                try
                {
                    client = await listener.AcceptTcpClientAsync(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch (SocketException) when (_listener is null || cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                _ = HandleClientSafelyAsync(client);
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "The remote command handler stopped unexpectedly.");
        }
        finally
        {
            if (listener is not null && ReferenceEquals(_listener, listener))
            {
                _listener = null;
            }

            listener?.Stop();
        }
    }

    /// <inheritdoc />
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.2.0</since>
    public void Stop()
    {
        if (_listener is null)
        {
            _logger.LogError("Cannot stop the remote command handler because it is not running.");

            return;
        }

        try
        {
            _listener.Stop();
            _listener = null;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to stop the remote command handler.");

            _listener = null;
        }

        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Stopped listening for remote commands.");
        }
    }

    /// <summary>
    /// Handles a remote client connection and logs unexpected failures instead of surfacing them through a background task.
    /// </summary>
    ///
    /// <param name="client">The connected remote client.</param>
    ///
    /// <returns>A task that represents the asynchronous client handling operation.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task HandleClientSafelyAsync(TcpClient client)
    {
        try
        {
            await HandleClientAsync(client);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to handle a remote command client.");

            client.Close();
        }
    }

    /// <summary>
    /// Validates a connected client, reads one remote command payload, and dispatches it when registered.
    /// </summary>
    ///
    /// <param name="client">The connected remote command client.</param>
    ///
    /// <returns>A task that completes when the client has been handled or rejected.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private async Task HandleClientAsync(TcpClient client)
    {
        var remoteEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
        IPAddress? remoteIp = remoteEndPoint?.Address;

        if (remoteIp == null || !_config.Whitelisted.Contains(remoteIp.ToString()))
        {
            if (_logger.IsEnabled(LogLevel.Warning))
            {
                _logger.LogWarning("Rejected connection from non-whitelisted address {Address:c}", remoteEndPoint);
            }

            client.Close();

            return;
        }

        await using NetworkStream stream = client.GetStream();

        RemoteCommandPayload? payload;

        try
        {
            payload = await ReadPayloadAsync(stream);
        }
        catch (EndOfStreamException)
        {
            if (_logger.IsEnabled(LogLevel.Warning))
            {
                _logger.LogWarning("Received incomplete payload from {Address:c}", remoteEndPoint);
            }

            return;
        }
        catch (JsonException)
        {
            if (_logger.IsEnabled(LogLevel.Warning))
            {
                _logger.LogWarning("Received malformed JSON payload from {Address:c}", remoteEndPoint);
            }

            return;
        }

        if (payload == null)
        {
            if (_logger.IsEnabled(LogLevel.Warning))
            {
                _logger.LogWarning("Received invalid payload from {Address:c}", remoteEndPoint);
            }

            return;
        }

        if (_commands.TryGetValue(payload.Command, out IInternalRemoteCommand? handler))
        {
            if (_logger.IsEnabled(LogLevel.Information) && _config.EnableReceiveLog)
            {
                _logger.LogInformation("Received remote command {Command:y} from {Address:c}", payload.Command, remoteEndPoint);
            }

            await handler.HandleRawAsync(payload.Data, stream);
        }
        else
        {
            if (_logger.IsEnabled(LogLevel.Warning) && _config.EnableReceiveLog)
            {
                _logger.LogWarning("Received unknown remote command {Command:y} from {Address:c}", payload.Command, remoteEndPoint);
            }
        }

        client.Close();
    }

    /// <summary>
    /// Reads a length-prefixed remote command payload from the network stream and deserializes it.
    /// </summary>
    ///
    /// <param name="stream">The network stream to read the payload from.</param>
    ///
    /// <returns>The deserialized <see cref="RemoteCommandPayload"/>, or <c>null</c> when the payload length is invalid.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.4.0</since>
    private static async Task<RemoteCommandPayload?> ReadPayloadAsync(NetworkStream stream)
    {
        byte[] lengthBuffer = new byte[sizeof(int)];
        await ReadExactlyAsync(stream, lengthBuffer);

        int length = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(lengthBuffer));

        if (length <= 0)
        {
            return null;
        }

        byte[] payloadBuffer = new byte[length];
        await ReadExactlyAsync(stream, payloadBuffer);

        string json = Encoding.UTF8.GetString(payloadBuffer);

        return JsonSerializer.Deserialize<RemoteCommandPayload>(json);
    }

    /// <summary>
    /// Reads exactly the specified number of bytes from the network stream into the provided buffer.
    /// </summary>
    ///
    /// <param name="stream">The network stream to read from.</param>
    /// <param name="buffer">The buffer to fill completely with the incoming data.</param>
    ///
    /// <returns>A task representing the asynchronous read operation.</returns>
    ///
    /// <exception cref="EndOfStreamException">Thrown when the stream closes before the full buffer has been read.</exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.4.0</since>
    private static async Task ReadExactlyAsync(NetworkStream stream, byte[] buffer)
    {
        var offset = 0;

        while (offset < buffer.Length)
        {
            int bytesRead = await stream.ReadAsync(buffer.AsMemory(offset));

            if (bytesRead == 0)
            {
                throw new EndOfStreamException("Remote command payload ended before the full message was received.");
            }

            offset += bytesRead;
        }
    }
}
