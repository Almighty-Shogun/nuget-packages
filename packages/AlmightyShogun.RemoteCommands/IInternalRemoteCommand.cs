using System.Text.Json;

namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Exposes the untyped contract used by the dispatcher to bind and execute remote commands.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>3.0.0</since>
internal interface IInternalRemoteCommand
{
    /// <summary>
    /// Binds the raw request payload to the command's message type.
    /// </summary>
    /// <param name="data">The raw request payload.</param>
    /// 
    /// <returns>The bound command message.</returns>
    /// 
    /// <author>Almighty-Shogun</author>
    /// <since>3.0.0</since>
    object Bind(JsonElement data);

    /// <summary>
    /// Executes the command with an already bound message.
    /// </summary>
    /// 
    /// <param name="message">The bound command message.</param>
    /// 
    /// <param name="response">The response writer for the command.</param>
    /// 
    /// <param name="cancellationToken">A token used to cancel command execution.</param>
    /// 
    /// <returns>A task that completes when command execution finishes.</returns>
    /// 
    /// <author>Almighty-Shogun</author>
    /// <since>3.0.0</since>
    Task ExecuteAsync(object message, ICommandResponse response, CancellationToken cancellationToken);
}
