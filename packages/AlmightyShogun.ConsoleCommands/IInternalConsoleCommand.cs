namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Exposes the command execution entry point used by the internal dispatcher.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>3.0.0</since>
internal interface IInternalConsoleCommand
{
    /// <summary>
    /// Executes the command after the dispatcher has parsed the command arguments.
    /// </summary>
    ///
    /// <param name="args">The raw argument values passed after the command name.</param>
    ///
    /// <returns>A task that completes when the command execution finishes.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>3.0.0</since>
    Task InternallyExecuteCommandAsync(string[] args);
}
