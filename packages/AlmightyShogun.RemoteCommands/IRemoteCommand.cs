namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Represents a remote command discovered during registration.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
internal interface IRemoteCommand
{
    /// <summary>
    /// Gets the command name.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    string Name { get; }
}

/// <summary>
/// Represents a remote command that handles messages of type <typeparamref name="T"/>.
/// </summary>
///
/// <typeparam name="T">
/// The command message type.
/// </typeparam>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
internal interface IRemoteCommand<in T> : IRemoteCommand where T : class
{
    /// <summary>
    /// Handles a remote command message.
    /// </summary>
    ///
    /// <param name="message">The command message.</param>
    /// <param name="response">
    /// The response writer for the command.
    /// </param>
    /// <param name="cancellationToken">
    /// A token used to cancel command execution.
    /// </param>
    ///
    /// <returns>A task that completes when command execution finishes.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    Task HandleCommandAsync(T message, ICommandResponse response, CancellationToken cancellationToken = default);
}
