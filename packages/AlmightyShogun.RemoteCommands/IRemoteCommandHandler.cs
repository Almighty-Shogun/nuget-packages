namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Controls the remote command listener.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
public interface IRemoteCommandHandler
{
    /// <summary>
    /// Starts the listener and serves requests until stopped or canceled.
    /// </summary>
    ///
    /// <param name="cancellationToken">
    /// A token used to stop the listener.
    /// </param>
    ///
    /// <returns>A task that completes when the listener has stopped.</returns>
    ///
    /// <remarks>
    /// Startup and listener failures are logged rather than propagated to the caller.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops the listener and active connections.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.2.0</since>
    void Stop();
}
