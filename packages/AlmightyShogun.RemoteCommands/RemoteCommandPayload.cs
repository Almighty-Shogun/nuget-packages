using System.Text.Json;

namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// The request frame a client sends: one command name, one message, and the key when the server asks for one.
/// </summary>
///
/// <remarks>
/// No property is <c>required</c>, so a frame that omits one deserializes rather than throwing and reaches the handler's
/// own guards: a missing name is refused with <see cref="RemoteCommandRefusal.MissingCommandName"/> and a missing message
/// with <see cref="RemoteCommandRefusal.InvalidMessage"/>. Marking either one required would collapse both onto
/// <see cref="RemoteCommandRefusal.MalformedPayload"/>, because deserialization would fail before the guards run.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
public sealed record RemoteCommandPayload
{
    /// <summary>
    /// Gets the name of the command to run, matched with ordinal case sensitivity. A blank name is refused with
    /// <see cref="RemoteCommandRefusal.MissingCommandName"/> before any command is looked up, which reaches the client as its
    /// numeric value because the serializer is configured with web defaults and no string enum converter.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public string Command { get; init; } = string.Empty;

    /// <summary>
    /// Gets the command's own message, left unbound here because only the command it is addressed to knows its type.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public JsonElement Data { get; init; }

    /// <summary>
    /// Gets the pre-shared key. Ignored when the server configures none, and compared in constant time when it does, so a
    /// wrong key cannot be found a character at a time.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public string? Secret { get; init; }
}
