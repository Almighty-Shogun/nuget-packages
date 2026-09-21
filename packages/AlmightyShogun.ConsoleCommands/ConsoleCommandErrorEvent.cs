namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Provides information about a failed console command invocation.
/// </summary>
///
/// <param name="commandName">The command name or alias that was invoked.</param>
/// <param name="exception">The exception that caused the command to fail.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed class ConsoleCommandErrorEvent(string commandName, Exception exception) : EventArgs
{
    /// <summary>
    /// Gets the command name or alias that was invoked.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public string CommandName { get; } = commandName;

    /// <summary>
    /// Gets the exception that caused the command to fail.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public Exception Exception { get; } = exception;
}
