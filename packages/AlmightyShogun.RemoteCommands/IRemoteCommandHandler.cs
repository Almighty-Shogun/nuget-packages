namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Controls the remote command TCP listener.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
public interface IRemoteCommandHandler
{
    /// <summary>
    /// Starts the TCP listener and dispatches accepted remote command payloads until cancellation or <see cref="Stop"/> is requested.
    /// </summary>
    ///
    /// <param name="cancellationToken">A token that cancels the listener loop.</param>
    ///
    /// <returns>A task that completes when the listener stops.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Requests the active TCP listener to stop accepting remote commands.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.2.0</since>
    void Stop();
}
