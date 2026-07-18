using System.Text.Json;
using System.Net.Sockets;

namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Exposes raw payload execution for the internal remote command dispatcher.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>3.0.0</since>
internal interface IInternalRemoteCommand
{
    /// <summary>
    /// Deserializes raw command data and invokes the typed command handler.
    /// </summary>
    ///
    /// <param name="data">The JSON element containing the command data.</param>
    /// <param name="stream">The network stream connected to the remote client.</param>
    ///
    /// <returns>A task that completes when the command has handled the payload.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>3.0.0</since>
    Task HandleRawAsync(JsonElement data, NetworkStream stream);
}
