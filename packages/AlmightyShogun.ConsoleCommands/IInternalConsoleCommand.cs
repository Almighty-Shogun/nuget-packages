using Microsoft.Extensions.Logging;

namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Exposes the execution entry point the dispatcher calls. Kept separate from <see cref="IConsoleCommand"/> so the
/// metadata a command advertises can be read without exposing a way to run it.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>3.0.0</since>
internal interface IInternalConsoleCommand
{
    /// <summary>
    /// Binds the typed arguments to the handler parameters and invokes it, or logs why it could not and returns without
    /// running anything. An argument that fails to convert stops the whole invocation rather than defaulting.
    /// </summary>
    ///
    /// <param name="args">The tokens typed after the command name, already split on whitespace and never <c>null</c>.</param>
    /// <param name="logger">
    /// The dispatcher's logger, supplied per invocation rather than held by the command, so a command that needs no
    /// services of its own can declare no constructor and still report bad input.
    /// </param>
    /// <param name="cancellationToken">
    /// Signalled when the handler is stopping. Passed on only to a handler whose last parameter is a
    /// <see cref="CancellationToken"/>; otherwise the command runs to completion regardless.
    /// </param>
    ///
    /// <returns>A task that completes when the handler has finished, or immediately when the arguments were rejected.</returns>
    ///
    /// <exception cref="Exception">
    /// Whatever the command's own <c>ExecuteAsync</c> threw, rethrown with its original stack trace rather than wrapped in
    /// <see cref="System.Reflection.TargetInvocationException"/>. Nothing on this path catches it.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>3.0.0</since>
    Task InternallyExecuteCommandAsync(string[] args, ILogger logger, CancellationToken cancellationToken);
}
