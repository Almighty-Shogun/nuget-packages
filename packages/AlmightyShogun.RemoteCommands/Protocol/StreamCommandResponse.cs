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
    /// Gets whether the command answered for itself, which is what stops a client waiting on a command that returned
    /// without writing and stops the dispatcher sending a second frame after one that did.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal bool HasWritten { get; private set; }

    /// <inheritdoc />
    public async Task WriteAsync<TResponse>(TResponse data, CancellationToken cancellationToken = default)
    {
        if (HasWritten)
            throw new InvalidOperationException("A response has already been written for this command.");

        RemoteCommandResponse envelope = new()
        {
            Data = JsonSerializer.SerializeToElement(data, RemoteCommandProtocol.SerializerOptions)
        };

        await RemoteCommandProtocol.WriteFrameAsync(stream, envelope, cancellationToken);

        HasWritten = true;
    }
}
