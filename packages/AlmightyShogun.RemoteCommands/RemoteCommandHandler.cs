using System.Net;
using System.Text;
using System.Text.Json;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AlmightyShogun.RemoteCommands;

public class RemoteCommandHandler : IRemoteCommandHandler
{
    private TcpListener? _listener;
    
    private readonly RemoteServerSettings _config;
    private readonly ILogger<RemoteCommandHandler> _logger;
    private readonly Dictionary<string, IInternalRemoteCommand> _commands;

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
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        _listener = new TcpListener(IPAddress.Parse(_config.Address), _config.Port);
        
        _listener.Start();

        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Started listening for remote command on {Address:c}:{Port:c}", _config.Address, _config.Port);
        }

        while (!cancellationToken.IsCancellationRequested)
        {
            TcpClient client = await _listener.AcceptTcpClientAsync(cancellationToken);
            
            _ = Task.Run(() => HandleClientAsync(client), cancellationToken);
        }
    }

    /// <inheritdoc />
    public void Stop()
    {
        if (_listener is null) return;
        
        _listener.Stop();
        _listener = null;
        
        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Stopped listening for remote commands.");
        }
    }
    
    /// <summary>
    /// Handles communication with a connected client by reading and processing remote commands.
    /// </summary>
    /// 
    /// <param name="client">The connected <see cref="TcpClient"/> representing the remote client.</param>
    /// 
    /// <returns>A task that represents the asynchronous handling of the client's communication.</returns>
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
    /// <returns>The deserialized <see cref="RemoteCommandPayload"/>, or null if the payload length is invalid.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.4.0</since>
    private static async Task<RemoteCommandPayload?> ReadPayloadAsync(NetworkStream stream)
    {
        var lengthBuffer = new byte[sizeof(int)];
        await ReadExactlyAsync(stream, lengthBuffer);

        int length = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(lengthBuffer));

        if (length <= 0)
        {
            return null;
        }

        var payloadBuffer = new byte[length];
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
