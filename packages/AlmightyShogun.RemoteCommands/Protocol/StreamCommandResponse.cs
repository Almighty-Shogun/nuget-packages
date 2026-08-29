using System.Text.Json;

namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Wraps a command's response in the standard envelope and writes it straight onto the connection, remembering that it
/// did so the dispatcher knows whether it still owes the client a frame.
/// </summary>
///
/// <param name="stream">
/// The connection to write to. Not owned: the connection outlives this response, because the same client may send
/// another request on it.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class StreamCommandResponse(Stream stream) : ICommandResponse
{
    /// <summary>
    /// Whether the write slot has been claimed, as an <see cref="int"/> so it can be tested and taken in one atomic step.
    /// <c>0</c> means the command still owes a frame, <c>1</c> means one caller has taken the slot.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private int _hasWritten;

    /// <summary>
    /// Gets whether the command answered for itself, which is what stops a client waiting on a command that returned
    /// without writing and stops the dispatcher sending a second frame after one that did.
    /// </summary>
    ///
    /// <remarks>
    /// Read through <see cref="Volatile"/> because the slot may have been claimed on another thread, and it turns
    /// <c>true</c> when a write begins rather than when it finishes. The dispatcher reads this only after the command has
    /// returned, by which point any write it started has completed or thrown.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal bool HasWritten => Volatile.Read(ref _hasWritten) != 0;

    /// <inheritdoc />
    public async Task WriteAsync<TResponse>(TResponse data, CancellationToken cancellationToken = default)
    {
        if (Interlocked.CompareExchange(ref _hasWritten, 1, 0) != 0)
            throw new InvalidOperationException("A response has already been written for this command.");

        RemoteCommandResponse envelope = new()
        {
            Data = JsonSerializer.SerializeToElement(data, RemoteCommandProtocol.SerializerOptions)
        };

        await RemoteCommandProtocol.WriteFrameAsync(stream, envelope, cancellationToken);
    }
}
