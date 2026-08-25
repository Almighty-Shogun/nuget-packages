using System.Text.Json;

namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// The request frame a client sends: one command name, one message, and the key when the server asks for one.
/// </summary>
///
/// <param name="Command">
/// The name of the command to run, matched with ordinal case sensitivity. A blank name is refused as
/// <c>missing_command</c> before any command is looked up.
/// </param>
/// <param name="Data">
/// The command's own message, left unbound here because only the command it is addressed to knows its type.
/// </param>
/// <param name="Secret">
/// The pre-shared key. Ignored when the server configures none, and compared in constant time when it does, so a wrong
/// key cannot be found a character at a time.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
public sealed record RemoteCommandPayload(string Command, JsonElement Data, string? Secret = null);
