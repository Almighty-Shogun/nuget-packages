namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Controls the TCP listener that accepts remote command requests. Registered as a singleton, and only one listener may
/// run on it at a time.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
public interface IRemoteCommandHandler
{
    /// <summary>
    /// Binds the configured address and port and serves requests until the token is canceled or <see cref="Stop"/> is
    /// called. Calling it while a listener is already running logs an error and returns rather than binding twice.
    /// </summary>
    ///
    /// <param name="cancellationToken">
    /// Stops the listener when signaled, which is how a hosted service hands over application shutdown. Connections
    /// already being served are given five seconds to drain before the method returns.
    /// </param>
    ///
    /// <returns>A task that completes once the listener has stopped and in-flight connections have drained or timed out.</returns>
    ///
    /// <remarks>
    /// A failure that reaches this method, such as the port already being in use, is logged rather than thrown, so a
    /// listener that never starts does not by itself bring the application down.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops accepting new connections and cancels the ones in flight. Calling it when no listener is running logs an
    /// error and returns, so it is safe from shutdown code that cannot know.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.2.0</since>
    void Stop();
}
