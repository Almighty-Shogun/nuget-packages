namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Writes the single response frame a command is allowed to send. Commands depend on this rather than on a
/// <c>NetworkStream</c>, so a command is testable with a fake and the transport can change without touching it.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public interface ICommandResponse
{
    /// <summary>
    /// Serializes a value and sends it as this request's response. A command that writes nothing is answered by the
    /// dispatcher instead, so calling this is optional but calling it twice is not allowed.
    /// </summary>
    ///
    /// <typeparam name="TResponse">
    /// The shape sent back. Serialized with the same web defaults the request was read with, so properties reach the
    /// client in camel case.
    /// </typeparam>
    /// <param name="data">
    /// The value to send. Carried as the response envelope's data rather than written bare, so a command whose own shape
    /// happens to have a <c>refusal</c> property is still read by the client as a success.
    /// </param>
    /// <param name="cancellationToken">Signaled when the read timeout elapses or the listener is stopping.</param>
    ///
    /// <returns>A task that completes once the frame has been written and flushed to the connection.</returns>
    ///
    /// <exception cref="InvalidOperationException">
    /// A response was already written for this request. The protocol is one frame per request, so a second write would
    /// be read by the client as the answer to whatever it sends next. The slot is claimed before the frame is written, so
    /// a write that fails still spends it and no retry is possible.
    /// </exception>
    /// <exception cref="IOException">The connection failed while writing the frame.</exception>
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was signaled mid-write.</exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Task WriteAsync<TResponse>(TResponse data, CancellationToken cancellationToken = default);
}
