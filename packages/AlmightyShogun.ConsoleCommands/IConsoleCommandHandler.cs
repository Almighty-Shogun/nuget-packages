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
    /// Starts reading console input and dispatching registered commands until cancellation or <see cref="Stop"/> is requested.
    /// </summary>
    ///
    /// <param name="cancellationToken">A token that cancels the command input loop.</param>
    ///
    /// <returns>A task that completes when the command input loop stops.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.1.0</since>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Requests the active console command input loop to stop.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    void Stop();
}
