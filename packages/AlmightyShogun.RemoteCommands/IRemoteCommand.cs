using System.Text.Json;
using System.Net.Sockets;

namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Exposes command metadata required by the internal remote command dispatcher.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
internal interface IRemoteCommand
{
    /// <summary>
    /// Gets the command name expected in incoming payloads.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    string Name { get; }
}

/// <summary>
/// Exposes the typed command handler contract implemented by remote command base classes.
/// </summary>
///
/// <typeparam name="T">The message type deserialized from the incoming command payload.</typeparam>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
internal interface IRemoteCommand<in T> : IRemoteCommand where T : class
{
    /// <summary>
    /// Handles the command with the deserialized message type.
    /// </summary>
    ///
    /// <param name="message">The deserialized command message.</param>
    /// <param name="stream">The network stream connected to the remote client.</param>
    ///
    /// <returns>A task that completes when the command has finished handling the message.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    Task HandleCommandAsync(T message, NetworkStream stream);
}
