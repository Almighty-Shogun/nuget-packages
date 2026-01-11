namespace AlmightyShogun.ConsoleCommands;

public interface IConsoleCommandHandler
{
    /// <summary>
    /// Starts the asynchronous operation for handling console commands.
    /// </summary>
    /// 
    /// <param name="cancellationToken"> A token to monitor for cancellation requests.</param>
    /// <returns>
    /// 
    /// A task that represents the asynchronous operation of starting the console command handler.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.1.0</since>
    Task StartAsync(CancellationToken cancellationToken = default);
}
