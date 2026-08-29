namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Carries a command failure to whoever subscribed to <see cref="IConsoleCommandHandler.CommandFailed"/>, for reporting
/// that the dispatcher's own logging cannot cover.
/// </summary>
///
/// <param name="commandName">The name as typed, which is the name that matched rather than the class behind it.</param>
/// <param name="exception">The exception the command let escape, with its original stack trace.</param>
///
/// <remarks>
/// Raised after the failure has already been logged, so a subscriber adds to the report rather than replacing it. The
/// prompt keeps reading whether anything is subscribed.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed class ConsoleCommandErrorEvent(string commandName, Exception exception) : EventArgs
{
    /// <summary>
    /// Gets the name the user typed. An alias resolves to the alias, not to the command's declared name, because it is
    /// what the person at the prompt actually wrote.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public string CommandName { get; } = commandName;

    /// <summary>
    /// Gets the exception the command threw. Already logged as an error by the dispatcher before this was raised.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public Exception Exception { get; } = exception;
}
