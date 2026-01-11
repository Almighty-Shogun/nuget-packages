namespace AlmightyShogun.ConsoleCommands;

[AttributeUsage(AttributeTargets.Method)]
public class ConsoleCommandAttribute(string name, string description, bool ignoreExtraArgs = false) : Attribute
{
    public string Name { get; } = name;
    public string Description { get; } = description;
    public bool IgnoreExtraArgs { get; } = ignoreExtraArgs;
}
