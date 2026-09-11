using System.Text.Json;

namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Represents a remote command request.
/// </summary>
///
/// <remarks>
/// No property is <c>required</c>, so a frame that omits one binds to the property's default rather than failing while
/// the frame is being read.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
public sealed record RemoteCommandPayload
{
    /// <summary>
    /// Gets the command name.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public string Command { get; init; } = string.Empty;

    /// <summary>
    /// Gets the command message data.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public JsonElement Data { get; init; }

    /// <summary>
    /// Gets the optional pre-shared key.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public string? Secret { get; init; }
}
