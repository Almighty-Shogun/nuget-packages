namespace AlmightyShogun.ConsoleCommands;

public class ConsoleCommand(string name, string description, string[] aliases, string usage, string? example)
{
    public string Name { get; } = name;
    public string Description { get; } = description;
    public string[] Aliases { get; } = aliases;
    public string? Usage { get; } = $"{name} {usage}";
    public string? Example { get; set; } = string.IsNullOrWhiteSpace(example) ? null : $"{name} {example}";
}
