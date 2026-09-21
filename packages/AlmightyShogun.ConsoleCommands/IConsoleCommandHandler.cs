namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Controls the console command input loop.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
public interface IConsoleCommandHandler
{
    /// <summary>
    /// Starts reading and dispatching console commands until cancellation or <see cref="Stop"/> is requested.
    /// </summary>
    ///
    /// <param name="cancellationToken">The token used to stop the command loop.</param>
    ///
    /// <returns>A task that completes when the command loop stops.</returns>
    ///
    /// <remarks>
    /// Only one command loop may run at a time. Command failures are reported through <see cref="CommandFailed"/> without
    /// stopping the loop.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.1.0</since>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Raised when a command fails during execution.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    event EventHandler<ConsoleCommandErrorEvent>? CommandFailed;

    /// <summary>
    /// Requests that the running command loop stop.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    void Stop();
}
