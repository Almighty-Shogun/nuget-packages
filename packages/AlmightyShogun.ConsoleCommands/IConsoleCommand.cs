namespace AlmightyShogun.ConsoleCommands;

public interface IConsoleCommand
{
    public string Name { get; }
    public string Description { get; }
    public IReadOnlyList<string> Aliases { get; }
    
    /// <summary>
    /// Executes the console command asynchronously based on the provided arguments.
    /// </summary>
    /// 
    /// <param name="args">An array of strings representing the arguments for the console command.</param>
    /// 
    /// <returns>A task representing the asynchronous command execution process.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    Task InternallyExecuteCommandAsync(string[] args);
}
