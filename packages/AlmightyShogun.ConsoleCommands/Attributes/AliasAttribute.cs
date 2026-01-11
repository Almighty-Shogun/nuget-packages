namespace AlmightyShogun.ConsoleCommands;

[AttributeUsage(AttributeTargets.Method)]
public class AliasAttribute(params string[] aliases) : Attribute
{
    public string[] Aliases { get; } = aliases;
}
