namespace AlmightyShogun.RemoteCommands;

[AttributeUsage(AttributeTargets.Class)]
public class RemoteCommandAttribute(string name, string? description) : Attribute
{
    public string Name { get; init; } = name;
    public string Description { get; set; } = description ?? string.Empty;
}
