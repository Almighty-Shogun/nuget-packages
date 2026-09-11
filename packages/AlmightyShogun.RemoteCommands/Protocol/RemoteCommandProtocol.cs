using System.Net;
using System.Text.Json;

namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Provides framing and serialization for remote command messages.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal static class RemoteCommandProtocol
{
    /// <summary>
    /// The serializer options used by the protocol.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    internal static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    /// <summary>
    /// Reads a length-prefixed message from a stream.
    /// </summary>
    ///
    /// <param name="stream">The stream to read from.</param>
    /// <param name="maxPayloadBytes">
    /// The maximum accepted payload size, in bytes.
    /// </param>
    /// <param name="cancellationToken">A token used to cancel the read.</param>
    ///
    /// <returns>
    /// The payload bytes, or <c>null</c> if the stream ends before a new frame begins.
    /// </returns>
    ///
    /// <exception cref="EndOfStreamException">
    /// The stream ended before the complete payload was received.
    /// </exception>
    /// <exception cref="InvalidDataException">
    /// The declared payload length is invalid or exceeds <paramref name="maxPayloadBytes"/>.
    /// </exception>
    /// <exception cref="IOException">The stream could not be read.</exception>
    /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    internal static async Task<byte[]?> ReadFrameAsync(Stream stream, int maxPayloadBytes, CancellationToken cancellationToken = default)
    {
        var lengthBuffer = new byte[sizeof(int)];

        if (!await TryReadExactlyAsync(stream, lengthBuffer, cancellationToken))
            return null;

        int length = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(lengthBuffer));

        if (length <= 0 || length > maxPayloadBytes)
            throw new InvalidDataException(
                $"Declared payload length {length} is outside the accepted range of 1 to {maxPayloadBytes} bytes.");

        var payload = new byte[length];

        if (!await TryReadExactlyAsync(stream, payload, cancellationToken))
            throw new EndOfStreamException("The connection ended before the full payload was received.");

        return payload;
    }

    /// <summary>
    /// Writes a length-prefixed serialized message to a stream.
    /// </summary>
    ///
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="stream">The stream to write to.</param>
    /// <param name="value">The value to serialize and write.</param>
    /// <param name="cancellationToken">A token used to cancel the write.</param>
    ///
    /// <returns>
    /// A task that completes when the message has been written.
    /// </returns>
    ///
    /// <exception cref="IOException">
    /// The stream could not be written.
    /// </exception>
    /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
    /// <exception cref="JsonException"><paramref name="value"/> could not be serialized.</exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    internal static async Task WriteFrameAsync<T>(Stream stream, T value, CancellationToken cancellationToken = default)
    {
        byte[] payload = JsonSerializer.SerializeToUtf8Bytes(value, SerializerOptions);
        byte[] lengthPrefix = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(payload.Length));

        await stream.WriteAsync(lengthPrefix, cancellationToken);
        await stream.WriteAsync(payload, cancellationToken);
        await stream.FlushAsync(cancellationToken);
    }

    /// <summary>
    /// Attempts to fill a buffer from a stream.
    /// </summary>
    ///
    /// <param name="stream">The stream to read from.</param>
    /// <param name="buffer">The buffer to fill.</param>
    /// <param name="cancellationToken">A token used to cancel the read.</param>
    ///
    /// <returns>
    /// <c>true</c> if the buffer was filled; <c>false</c> if the stream ended before any bytes were read.
    /// </returns>
    ///
    /// <exception cref="EndOfStreamException">The stream ended after only part of the buffer was filled.</exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
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
