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
    private const int _maxPayloadBytes = 1024 * 1024;

    /// <summary>
    /// The connection, opened on first use and reused afterward. Discarded on every path that leaves it unusable or out
    /// of step: a transport failure, a canceled wait, a server that closed without sending a frame, a framing error, and
    /// a frame that is not a readable envelope. The next call then reconnects rather than writing into a broken socket.
    /// A response body that does not bind to the caller's type is not one of those, since the frame was read in full and
    /// the connection is still in step.
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
    /// <exception cref="RemoteCommandProtocolException">The frame deserialized to <c>null</c>, so it carried no envelope.</exception>
    /// <exception cref="RemoteCommandRefusedException">
    /// The server answered and declined. Its <see cref="RemoteCommandRefusedException.Reason"/> says what it objected to,
    /// and reports <see cref="RemoteCommandRefusal.Other"/> for a reason this client does not know.
    /// </exception>
    /// <exception cref="InvalidDataException">
    /// The frame's declared length is unusable or exceeds the one megabyte cap. The connection is discarded, so the next
    /// call opens a fresh one.
    /// </exception>
    /// <exception cref="JsonException">
    /// The frame is not valid JSON for an envelope, in which case the connection is discarded, or the envelope's data does
    /// not bind to <typeparamref name="TResponse"/>, in which case it is kept because the frame was read in full. Neither
    /// is a <see cref="RemoteCommandException"/>, so a caller catching only that type does not see it.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// <paramref name="cancellationToken"/> was signaled. Rethrown as-is rather than wrapped, though the connection is
    /// discarded first.
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

            frame = await RemoteCommandProtocol.ReadFrameAsync(stream, _maxPayloadBytes, cancellationToken);
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
    /// Which subclass is thrown says whether the server refused the command, could not be reached, or closed the
    /// connection without answering. Only <see cref="RemoteCommandUnreachableException"/> and
    /// <see cref="RemoteCommandRefusedException"/> mean the command did not run: the server runs a command before it
    /// writes, so a disconnection can also mean it ran and the answer never came back.
    /// </exception>
    /// <exception cref="InvalidDataException">The frame was unusable. Carries the same meaning as on the overload this calls.</exception>
    /// <exception cref="JsonException">
    /// The frame was not a valid envelope. Carries the same meaning as on the overload this calls.
    /// </exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was signaled.</exception>
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
