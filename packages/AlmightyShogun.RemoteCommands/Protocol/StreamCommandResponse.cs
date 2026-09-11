using System.Text.Json;

namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Writes command responses to a stream.
/// </summary>
///
/// <param name="stream">
/// The stream to write responses to.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal sealed class StreamCommandResponse(Stream stream) : ICommandResponse
{
    /// <summary>
    /// Tracks whether the response slot has been claimed.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private int _hasWritten;

    /// <summary>
    /// Gets whether a response has been written for the current command.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    internal bool HasWritten => Volatile.Read(ref _hasWritten) != 0;

    /// <inheritdoc />
    public async Task WriteAsync<TResponse>(TResponse data, CancellationToken cancellationToken = default)
    {
        RemoteCommandResponse envelope = new()
        {
            Data = JsonSerializer.SerializeToElement(data, RemoteCommandProtocol.SerializerOptions)
        };
        
        if (Interlocked.CompareExchange(ref _hasWritten, 1, 0) != 0)
            throw new InvalidOperationException("A response has already been written for this command.");

        await RemoteCommandProtocol.WriteFrameAsync(stream, envelope, cancellationToken);
    }
}
