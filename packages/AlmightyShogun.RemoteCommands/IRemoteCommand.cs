namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Exposes the name a command answers to, which is the only thing the dispatcher needs before it decides whether a
/// request belongs to this command at all. Every command is registered under this type so the table can be built once.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
internal interface IRemoteCommand
{
    /// <summary>
    /// Gets the name matched against the <c>command</c> field of an incoming frame, compared with ordinal case
    /// sensitivity, so the wire name must be written exactly as the attribute declares it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    string Name { get; }
}

/// <summary>
/// Exposes the handler a command implements once its payload has been turned into <typeparamref name="T"/>. Split from
/// the untyped contract so the dispatcher can hold every command in one table without knowing any message type.
/// </summary>
///
/// <typeparam name="T">
/// The message the command expects. Bound from the request's <c>data</c> field, so its shape is the command's wire
/// contract as much as the command name is.
/// </typeparam>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
internal interface IRemoteCommand<in T> : IRemoteCommand where T : class
{
    /// <summary>
    /// Runs the command against a message that has already been deserialized and checked for null.
    /// </summary>
    ///
    /// <param name="message">The bound message, never <c>null</c>.</param>
    /// <param name="response">
    /// The writer for this request, usable exactly once. Leaving it unused is allowed: the dispatcher then sends its own
    /// acknowledgement so the client is never left waiting for a frame that never comes.
    /// </param>
    /// <param name="cancellationToken">
    /// Signaled when the read timeout elapses or the listener is stopping, so a long command is abandoned rather than
    /// holding a connection slot.
    /// </param>
    ///
    /// <returns>A task that completes when the command has finished, whether or not it wrote a response.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    Task HandleCommandAsync(T message, ICommandResponse response, CancellationToken cancellationToken = default);
}
