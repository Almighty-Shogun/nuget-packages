namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Controls the console input loop that reads lines and dispatches them to the registered commands. Only one loop may run
/// on it at a time.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
public interface IConsoleCommandHandler
{
    /// <summary>
    /// Reads and dispatches console input until the token is canceled or <see cref="Stop"/> is called. Calling it while a
    /// loop is already running logs an error and returns rather than starting a second reader on the same console.
    /// </summary>
    ///
    /// <param name="cancellationToken">
    /// Stops the loop when signaled, which is how a hosted service hands over application shutdown. A command already
    /// running is not interrupted unless it accepts the token itself.
    /// </param>
    ///
    /// <returns>A task that completes once the loop has stopped and the command in flight, if any, has finished.</returns>
    ///
    /// <remarks>
    /// An exception escaping a command is logged and the prompt keeps reading, so one failing command does not take the
    /// console down with it. Subscribe to <see cref="CommandFailed"/> to report it anywhere else. An exception that escapes
    /// the dispatch of a line rather than the command itself ends the loop and is logged as an unexpected stop.
    ///
    /// The loop also ends when the input stream does. A redirected process reaching end of input stops rather than
    /// spinning on a reader that will never return another line.
    ///
    /// Starting sets <c>Console.TreatControlCAsInput</c> to <c>false</c> for the whole process, so Ctrl+C is handled as an
    /// interrupt instead of being delivered to the reader as a line. It is never restored, and an <see cref="IOException"/>
    /// from a console that does not support the write is swallowed.
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
    /// Handlers run on the loop's thread before the next line is read, so a slow one delays the prompt. An exception from a
    /// handler escapes into <see cref="StartAsync"/> rather than being contained the way the command's own failure was.
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
