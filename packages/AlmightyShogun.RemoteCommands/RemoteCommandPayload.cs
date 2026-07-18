using System.Text.Json;

namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Represents the wire payload received by the remote command listener.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
internal sealed class RemoteCommandPayload
{
    /// <summary>
    /// Gets the command name used to select a registered handler.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public required string Command { get; init; }

    /// <summary>
    /// Gets the raw JSON data passed to the selected command handler.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public JsonElement Data { get; init; }
}
