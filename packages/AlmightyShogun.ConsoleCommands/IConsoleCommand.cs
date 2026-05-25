namespace AlmightyShogun.ConsoleCommands;

public interface IConsoleCommand
{
    public string Name { get; }
    public string? Description { get; }
    public IReadOnlyList<string> Aliases { get; }
}
