namespace AlmightyShogun.RemoteCommands;

public interface IRemoteCommandHandler
{
    /// <summary>
    /// Starts the asynchronous listener for handling remote commands.
    /// </summary>
    /// 
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// 
    /// <returns>A task that represents the asynchronous operation.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops the listener from handling remote commands.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.2.0</since>
    void Stop();
}
