using Microsoft.Extensions.Logging;

namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Defines the internal execution entry point used to run a console command.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>3.0.0</since>
internal interface IInternalConsoleCommand
{
    /// <summary>
    /// Binds the supplied arguments and invokes the command handler.
    /// </summary>
    ///
    /// <param name="args">The parsed command arguments.</param>
    /// <param name="logger">The logger used to report argument and execution errors.</param>
    /// <param name="cancellationToken">The cancellation token forwarded to handlers that accept one.</param>
    ///
    /// <returns>A task that completes when command execution finishes.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>3.0.0</since>
    Task InternallyExecuteCommandAsync(string[] args, ILogger logger, CancellationToken cancellationToken);
}
