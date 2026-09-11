using System.Text.Json;
using System.Net.Sockets;

namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Sends remote commands to a remote command server.
/// </summary>
///
/// <param name="host">The server host.</param>
/// <param name="port">The server port.</param>
/// <param name="secret">The optional pre-shared key sent with requests.</param>
/// <param name="maxPayloadBytes">The maximum accepted response payload size, in bytes.</param>
///
/// <remarks>
/// Requests are sent sequentially over a reusable connection. This type is not safe for concurrent use.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed class RemoteCommandClient(
    string host,
    int port,
    string? secret = null,
    int maxPayloadBytes = 1024 * 1024
) : IAsyncDisposable
{

    /// <summary>
    /// The current TCP connection.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private TcpClient? _client;

    /// <summary>
    /// The stream for the current connection.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private NetworkStream? _stream;

    /// <summary>
    /// Sends a command and waits for its response.
    /// </summary>
    ///
    /// <typeparam name="TMessage">The command message type.</typeparam>
    /// <typeparam name="TResponse">The expected response type.</typeparam>
    /// <param name="command">The command name.</param>
    /// <param name="message">The command message.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    ///
    /// <returns>The response, or <c>default</c> if no response data was returned.</returns>
    ///
    /// <exception cref="RemoteCommandUnreachableException">The server could not be reached.</exception>
    /// <exception cref="RemoteCommandDisconnectedException">
    /// The connection closed before a response was received.
    /// </exception>
    /// <exception cref="RemoteCommandProtocolException">The server returned an invalid response envelope.</exception>
    /// <exception cref="RemoteCommandRefusedException">
    /// The server refused the command.
    /// </exception>
    /// <exception cref="InvalidDataException">
    /// The response frame length is invalid or exceeds the configured maximum.
    /// </exception>
    /// <exception cref="JsonException">
    /// The request or response could not be serialized or deserialized.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// The operation was canceled.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public async Task<TResponse?> SendAsync<TMessage, TResponse>(
        string command,
        TMessage message,
        CancellationToken cancellationToken = default
    )
    {
        byte[]? frame;

        try
        {
            NetworkStream stream = await ConnectAsync(cancellationToken);

            RemoteCommandPayload payload = new()
            {
                Command = command,
                Data = JsonSerializer.SerializeToElement(message, RemoteCommandProtocol.SerializerOptions),
                Secret = secret
            };

            await RemoteCommandProtocol.WriteFrameAsync(stream, payload, cancellationToken);

            frame = await RemoteCommandProtocol.ReadFrameAsync(stream, maxPayloadBytes, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            await DisposeAsync();

            throw;
        }
        catch (SocketException exception)
        {
            await DisposeAsync();

            throw new RemoteCommandUnreachableException(host, port, exception);
        }
        catch (Exception exception) when (exception is IOException or EndOfStreamException)
        {
            await DisposeAsync();

            throw new RemoteCommandDisconnectedException(exception);
        }
        catch (InvalidDataException)
        {
            await DisposeAsync();

            throw;
        }

        if (frame is null)
        {
            await DisposeAsync();

            throw new RemoteCommandDisconnectedException();
        }

        RemoteCommandResponse? envelope;

        try
        {
            envelope = JsonSerializer.Deserialize<RemoteCommandResponse>(frame, RemoteCommandProtocol.SerializerOptions);
        }
        catch (JsonException)
        {
            await DisposeAsync();

            throw;
        }

        if (envelope is null)
        {
            await DisposeAsync();

            throw new RemoteCommandProtocolException("The server sent a frame that is not a response envelope.");
        }

        if (envelope.Refusal is { } refusal)
            throw new RemoteCommandRefusedException(Enum.IsDefined(refusal) ? refusal : RemoteCommandRefusal.Other);

        return envelope.Data is { } data
            ? data.Deserialize<TResponse>(RemoteCommandProtocol.SerializerOptions)
            : default;
    }

    /// <summary>
    /// Sends a command and waits for the server to acknowledge it.
    /// </summary>
    ///
    /// <typeparam name="TMessage">The command message type.</typeparam>
    /// <param name="command">The command name.</param>
    /// <param name="message">The command message.</param>
    /// <param name="cancellationToken">A token used to cancel the operation.</param>
    ///
    /// <returns>
    /// A task that completes when the server responds.
    /// </returns>
    ///
    /// <exception cref="RemoteCommandException">
    /// A remote command error occurred.
    /// </exception>
    /// <exception cref="InvalidDataException">The response frame is invalid.</exception>
    /// <exception cref="JsonException">
    /// The request or response could not be serialized or deserialized.
    /// </exception>
    /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public Task SendAsync<TMessage>(string command, TMessage message, CancellationToken cancellationToken = default)
        => SendAsync<TMessage, JsonElement>(command, message, cancellationToken);

    /// <summary>
    /// Closes the current connection.
    /// </summary>
    ///
    /// <returns>A task that completes when the connection has been disposed.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public async ValueTask DisposeAsync()
    {
        if (_stream is not null)
            await _stream.DisposeAsync();

        _client?.Dispose();

        _stream = null;
        _client = null;
    }

    /// <summary>
    /// Gets the current connection stream, connecting if necessary.
    /// </summary>
    ///
    /// <param name="cancellationToken">A token used to cancel the connection attempt.</param>
    ///
    /// <returns>
    /// The active connection stream.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private async Task<NetworkStream> ConnectAsync(CancellationToken cancellationToken)
    {
        if (_stream is not null && _client?.Connected == true)
            return _stream;

        await DisposeAsync();

        _client = new TcpClient();

        await _client.ConnectAsync(host, port, cancellationToken);

        _stream = _client.GetStream();

        return _stream;
    }
}
