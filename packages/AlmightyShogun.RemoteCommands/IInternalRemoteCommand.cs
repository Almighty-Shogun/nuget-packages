using System.Text.Json;

namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Exposes the untyped entry point the dispatcher calls, which is what lets one table hold commands whose message types
/// have nothing in common. The generic base implements it by binding the payload and forwarding.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>3.0.0</since>
internal interface IInternalRemoteCommand
{
    /// <summary>
    /// Binds the request payload to the command's message type and runs it.
    /// </summary>
    ///
    /// <param name="data">
    /// The <c>data</c> field of the request frame exactly as it arrived, still unbound because only the command knows
    /// what type it should become.
    /// </param>
    /// <param name="response">The writer for this request, usable exactly once.</param>
    /// <param name="cancellationToken">Signaled when the read timeout elapses or the listener is stopping.</param>
    ///
    /// <returns>A task that completes when the command has finished.</returns>
    ///
    /// <exception cref="JsonException">
    /// The payload could not become the command's message type. The dispatcher answers with a
    /// <see cref="RemoteCommandRefusal.InvalidMessage"/> refusal, unless the command had already claimed the write slot, in
    /// which case the refusal is suppressed and the client is left waiting.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>3.0.0</since>
    Task HandleRawAsync(JsonElement data, ICommandResponse response, CancellationToken cancellationToken);
}
