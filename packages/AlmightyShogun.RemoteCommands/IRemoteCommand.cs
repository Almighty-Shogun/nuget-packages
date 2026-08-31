namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// The marker the assembly scan looks for, so a command is found without the scan knowing its message type. Registration
/// adds each command under its concrete type rather than under this interface, and builds the dispatch table from the
/// <see cref="RemoteCommandDescriptor"/> singletons it registers alongside them, so nothing resolves this type at all.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
internal interface IRemoteCommand
{
    /// <summary>
    /// Gets the name the command declares, which <see cref="RemoteCommand{T}"/> reads from
    /// <see cref="RemoteCommandAttribute"/> once per instance. Routing does not read it: the dispatch table is keyed by
    /// <see cref="RemoteCommandDescriptor.Name"/>, taken from the same attribute at registration.
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
