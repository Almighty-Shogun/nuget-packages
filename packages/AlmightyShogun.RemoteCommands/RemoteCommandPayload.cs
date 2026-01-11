using System.Text.Json;

namespace AlmightyShogun.RemoteCommands;

public class RemoteCommandPayload
{
    public required string Command { get; init; }
    public JsonElement Data { get; init; }
}
