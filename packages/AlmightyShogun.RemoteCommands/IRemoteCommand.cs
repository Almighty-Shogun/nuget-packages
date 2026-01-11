using System.Text.Json;
using System.Net.Sockets;

namespace AlmightyShogun.RemoteCommands;

public interface IRemoteCommand
{
    public string Name { get; }

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
    public Task HandleRawAsync(JsonElement data, NetworkStream stream);
}

public interface IRemoteCommand<in T> : IRemoteCommand where T : class
{
    /// <summary>
    /// Handles the command with the structured message type.
    /// </summary>
    /// 
    /// <param name="message">The structured message containing the command data.</param>
    /// <param name="stream">The network stream used for writing the command.</param>
    /// 
    /// <returns>A task that represents the asynchronous operation.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    Task HandleCommandAsync(T message, NetworkStream stream);
}