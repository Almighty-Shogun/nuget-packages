using System.Net;
using System.Buffers;
using System.Text.Json;

namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Owns the wire format: a big-endian four-byte length prefix followed by that many bytes of UTF-8 JSON.
/// </summary>
///
/// <remarks>
/// Both the server and <see cref="RemoteCommandClient"/> use this type, which is what stops the two sides of the wire
/// drifting apart. A framing mismatch surfaces only as an unreadable payload, with nothing indicating which side is wrong.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class RemoteCommandProtocol
{
    /// <summary>
    /// The serializer options used for every frame in both directions. Web defaults give camel-case output and
    /// case-insensitive matching, so both casings are accepted on the wire.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    /// <summary>
    /// Reads one whole message, blocking until every declared byte has arrived, so a caller never sees a partial frame.
    /// </summary>
    ///
    /// <param name="stream">The connection to read from, left open for the next message.</param>
    /// <param name="maxPayloadBytes">
    /// The largest payload accepted. Checked against the declared length before a buffer is rented, so a hostile length
    /// prefix cannot make the server allocate on its behalf.
    /// </param>
    /// <param name="cancellationToken">Signaled when the idle timeout elapses or the listener is stopping.</param>
    ///
    /// <returns>
    /// The payload bytes, or <c>null</c> when the peer closed the connection between messages, which is the ordinary way
    /// a client goes away and not an error.
    /// </returns>
    ///
    /// <exception cref="EndOfStreamException">
    /// The connection ended part-way through a message, which means the peer died rather than disconnected.
    /// </exception>
    /// <exception cref="InvalidDataException">
    /// The declared length was zero, negative, or above the accepted maximum, so the frame is unreadable and the
    /// connection can no longer be trusted to be in sync.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal static async Task<byte[]?> ReadFrameAsync(Stream stream, int maxPayloadBytes, CancellationToken cancellationToken = default)
    {
        var lengthBuffer = new byte[sizeof(int)];

        if (!await TryReadExactlyAsync(stream, lengthBuffer, cancellationToken))
            return null;

        int length = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(lengthBuffer));

        if (length <= 0 || length > maxPayloadBytes)
            throw new InvalidDataException(
                $"Declared payload length {length} is outside the accepted range of 1 to {maxPayloadBytes} bytes."
            );

        byte[] rented = ArrayPool<byte>.Shared.Rent(length);

        try
        {
            Memory<byte> payload = rented.AsMemory(0, length);

            if (!await TryReadExactlyAsync(stream, payload, cancellationToken))
                throw new EndOfStreamException("The connection ended before the full payload was received.");

            return payload.ToArray();
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(rented);
        }
    }

    /// <summary>
    /// Writes one whole message as a length prefix followed by the body, flushing so the frame has actually left before
    /// the caller waits for an answer to it.
    /// </summary>
    ///
    /// <typeparam name="T">The value's type, serialized with the shared web defaults.</typeparam>
    /// <param name="stream">The connection to write to, left open for the next message.</param>
    /// <param name="value">The value to send as the frame body.</param>
    /// <param name="cancellationToken">Signaled when the read timeout elapses or the listener is stopping.</param>
    ///
    /// <returns>
    /// A task that completes once the length prefix and body have both been written and flushed, so a caller awaiting it
    /// knows the whole frame has left rather than half of it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal static async Task WriteFrameAsync<T>(Stream stream, T value, CancellationToken cancellationToken = default)
    {
        byte[] payload = JsonSerializer.SerializeToUtf8Bytes(value, SerializerOptions);
        byte[] lengthPrefix = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(payload.Length));

        await stream.WriteAsync(lengthPrefix, cancellationToken);
        await stream.WriteAsync(payload, cancellationToken);
        await stream.FlushAsync(cancellationToken);
    }

    /// <summary>
    /// Fills a buffer completely, or reports that the stream ended first.
    /// </summary>
    ///
    /// <param name="stream">The connection to read from.</param>
    /// <param name="buffer">The buffer to fill completely, whose length is how many bytes are expected.</param>
    /// <param name="cancellationToken">Signaled when the idle timeout elapses or the listener is stopping.</param>
    ///
    /// <returns>
    /// <c>true</c> when the buffer was filled, and <c>false</c> when the peer closed before sending anything. The
    /// distinction matters: nothing read is a clean disconnect, some read is a truncated message.
    /// </returns>
    ///
    /// <exception cref="EndOfStreamException">The connection ended after part of the expected bytes had arrived.</exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static async Task<bool> TryReadExactlyAsync(Stream stream, Memory<byte> buffer, CancellationToken cancellationToken)
    {
        var offset = 0;

        while (offset < buffer.Length)
        {
            int bytesRead = await stream.ReadAsync(buffer[offset..], cancellationToken);

            if (bytesRead == 0)
                return offset == 0
                    ? false
                    : throw new EndOfStreamException("The connection ended before the full message was received.");

            offset += bytesRead;
        }

        return true;
    }
}
