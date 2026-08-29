namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Controls the console input loop that reads lines and dispatches them to the registered commands. Registered as a
/// singleton, and only one loop may run on it at a time.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
public interface IConsoleCommandHandler
{
    /// <summary>
    /// Reads and dispatches console input until the token is cancelled or <see cref="Stop"/> is called. Calling it while a
    /// loop is already running logs an error and returns rather than starting a second reader on the same console.
    /// </summary>
    ///
    /// <param name="cancellationToken">
    /// Stops the loop when signalled, which is how a hosted service hands over application shutdown. A command already
    /// running is not interrupted unless it accepts the token itself.
    /// </param>
    ///
    /// <returns>A task that completes once the loop has stopped and the command in flight, if any, has finished.</returns>
    ///
    /// <remarks>
    /// An exception escaping a command is logged and the prompt keeps reading, so one failing command does not take the
    /// console down with it. Subscribe to <see cref="CommandFailed"/> to report it anywhere else.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.1.0</since>
    Task StartAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Raised after a command threw and the failure was logged, for reporting it somewhere the dispatcher knows nothing
    /// about, such as telemetry or a non-zero exit code.
    /// </summary>
    ///
    /// <remarks>
    /// Handlers run on the loop's thread before the next line is read, so a slow one delays the prompt. An exception from
    /// a handler is not caught, and would take down the loop that the command itself was not allowed to.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    event EventHandler<ConsoleCommandErrorEvent>? CommandFailed;

    /// <summary>
    /// Asks the running loop to stop, which is what a command such as <c>exit</c> calls on itself. Calling it when no loop
    /// is running logs an error and returns, so it is safe to call from shutdown code that cannot know.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    void Stop();
}
