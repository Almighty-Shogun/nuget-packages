using System.Text.Json;
using System.Net.Sockets;

namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Sends remote commands to a listener, using the same framing the server reads.
/// </summary>
///
/// <param name="host">The listener host, resolved on first use rather than at construction.</param>
/// <param name="port">The listener port.</param>
/// <param name="secret">
/// The pre-shared key to send with every request. Leave it <c>null</c> against a server that requires none; sending one
/// the server does not ask for is ignored rather than refused.
/// </param>
///
/// <remarks>
/// The connection stays open, so several commands can be sent in sequence without reconnecting. Requests are sequential:
/// one request, one response, then the next. Not safe for concurrent use, because two callers would interleave frames on
/// the same socket and each read the other's response.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed class RemoteCommandClient(string host, int port, string? secret = null) : IAsyncDisposable
{
    /// <summary>
    /// The largest response frame accepted, matching the server's own default. A server configured to allow more can
    /// send a response this client will refuse to read.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private const int MaxPayloadBytes = 1024 * 1024;

    /// <summary>
    /// The connection, opened on first use and reused afterwards. Discarded on any transport failure so the next call
    /// reconnects rather than writing into a broken socket.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private TcpClient? _client;

    /// <summary>
    /// The stream over the current connection, held alongside it because both have to be disposed together.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private NetworkStream? _stream;

    /// <summary>
    /// Sends one command and waits for the single frame that answers it, opening the connection first if this is the
    /// first call or the previous one failed.
    /// </summary>
    ///
    /// <typeparam name="TMessage">The message type, which must match what the command declares on the server.</typeparam>
    /// <typeparam name="TResponse">The shape expected back, bound from the response frame.</typeparam>
    /// <param name="command">The command name, matched case-sensitively by the server.</param>
    /// <param name="message">The message to send as the request's data.</param>
    /// <param name="cancellationToken">Cancels the send and the wait for a response.</param>
    ///
    /// <returns>
    /// The response, or <c>default</c> when the command ran but wrote nothing of its own and the server acknowledged it
    /// instead.
    /// </returns>
    ///
    /// <exception cref="RemoteCommandUnreachableException">The connection could not be opened, so nothing was sent.</exception>
    /// <exception cref="RemoteCommandDisconnectedException">
    /// The connection closed before a response arrived, which usually means this address is not whitelisted.
    /// </exception>
    /// <exception cref="RemoteCommandProtocolException">A frame arrived that is not a response envelope.</exception>
    /// <exception cref="RemoteCommandRefusedException">
    /// The server answered and declined. Its <see cref="RemoteCommandRefusedException.Reason"/> says what it objected to,
    /// and reports <see cref="RemoteCommandRefusal.Other"/> for a reason this client does not know.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
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

            RemoteCommandPayload payload = new(
                command,
                JsonSerializer.SerializeToElement(message, RemoteCommandProtocol.SerializerOptions),
                secret
            );

            await RemoteCommandProtocol.WriteFrameAsync(stream, payload, cancellationToken);

            frame = await RemoteCommandProtocol.ReadFrameAsync(stream, MaxPayloadBytes, cancellationToken);
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

        if (frame is null)
        {
            await DisposeAsync();

            throw new RemoteCommandDisconnectedException();
        }

        var envelope = JsonSerializer.Deserialize<RemoteCommandResponse>(frame, RemoteCommandProtocol.SerializerOptions);

        if (envelope is null)
            throw new RemoteCommandProtocolException("The server sent a frame that is not a response envelope.");

        if (envelope.Refusal is { } refusal)
            throw new RemoteCommandRefusedException(Enum.IsDefined(refusal) ? refusal : RemoteCommandRefusal.Other);

        return envelope.Data is { } data
            ? data.Deserialize<TResponse>(RemoteCommandProtocol.SerializerOptions)
            : default;
    }

    /// <summary>
    /// Sends a command whose response is not wanted, still waiting for the answer because the frame has to leave the
    /// connection before the next request can be read.
    /// </summary>
    ///
    /// <typeparam name="TMessage">The message type, which must match what the command declares on the server.</typeparam>
    /// <param name="command">The command name, matched case-sensitively by the server.</param>
    /// <param name="message">The message to send as the request's data.</param>
    /// <param name="cancellationToken">Cancels the send and the wait for a response.</param>
    ///
    /// <returns>
    /// A task that completes when the server has answered. Any response body is read and discarded, because the frame
    /// still has to leave the connection for the next request to be readable.
    /// </returns>
    ///
    /// <exception cref="RemoteCommandException">
    /// The command did not run. Which subclass is thrown says whether the server refused it, could not be reached, or
    /// closed the connection without answering.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public Task SendAsync<TMessage>(string command, TMessage message, CancellationToken cancellationToken = default)
        => SendAsync<TMessage, JsonElement>(command, message, cancellationToken);

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_stream is not null)
            await _stream.DisposeAsync();

        _client?.Dispose();

        _stream = null;
        _client = null;
    }

    /// <summary>
    /// Opens the connection on first use and reuses it afterwards.
    /// </summary>
    ///
    /// <param name="cancellationToken">Cancels the connection attempt.</param>
    ///
    /// <returns>
    /// The stream for the live connection, whether it was already open or has just been established.
    /// </returns>
    ///
    /// <exception cref="SocketException">
    /// The host could not be reached. Left unwrapped here and turned into a
    /// <see cref="RemoteCommandUnreachableException"/> by the caller, which is the only path that reaches consumers.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
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
