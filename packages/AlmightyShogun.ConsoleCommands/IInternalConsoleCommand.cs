namespace AlmightyShogun.ConsoleCommands;

internal interface IInternalConsoleCommand
{
    /// <summary>
    /// Handles the execution of a console command with the provided arguments.
    /// </summary>
    /// 
    /// <param name="args">An array of strings representing the arguments passed to the command. Arguments must match the command's parameter configuration.</param>
    ///
    /// <returns>A task that represents the asynchronous operation of handling the command.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    Task InternallyExecuteCommandAsync(string[] args);
}
