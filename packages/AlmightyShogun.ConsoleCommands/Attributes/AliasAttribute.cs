namespace AlmightyShogun.ConsoleCommands;

[AttributeUsage(AttributeTargets.Class)]
public class AliasAttribute(params string[] aliases) : Attribute
{
    public string[] Aliases { get; } = aliases;
}
