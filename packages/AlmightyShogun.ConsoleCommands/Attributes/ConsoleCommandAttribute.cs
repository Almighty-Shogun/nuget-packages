namespace AlmightyShogun.ConsoleCommands;

[AttributeUsage(AttributeTargets.Class)]
public class ConsoleCommandAttribute(string name, string? description = null, bool ignoreExtraArgs = false) : Attribute
{
    public string Name { get; } = name;
    public string? Description { get; } = description;
    public bool IgnoreExtraArgs { get; } = ignoreExtraArgs;
}
