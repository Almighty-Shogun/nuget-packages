using System.Text.Json;

namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// The request frame a client sends: one command name, one message, and the key when the server asks for one.
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
    /// The name of the command to run, as declared on that command's <see cref="RemoteCommandAttribute"/>.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public string Command { get; init; } = string.Empty;

    /// <summary>
    /// The command's own message, left unbound here because only the command it is addressed to knows its type.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public JsonElement Data { get; init; }

    /// <summary>
    /// The pre-shared key sent with the request, or <c>null</c> when the client sends none.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public string? Secret { get; init; }
}
