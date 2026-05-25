using System.Text.Json;
using System.Net.Sockets;

namespace AlmightyShogun.RemoteCommands;

internal interface IInternalRemoteCommand
{
    /// <summary>
    /// Handles the command with the raw JSON data and invokes the corresponding command handler.
    /// </summary>
    /// 
    /// <param name="data">The JSON element containing the raw data for the command.</param>
    /// <param name="stream">The network stream used for transmitting the command.</param>
    /// 
    /// <returns>A task that represents the asynchronous handling of the raw data.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    Task HandleRawAsync(JsonElement data, NetworkStream stream);
}
