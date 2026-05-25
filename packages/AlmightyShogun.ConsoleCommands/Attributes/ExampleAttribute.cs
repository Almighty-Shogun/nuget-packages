namespace AlmightyShogun.ConsoleCommands;

[AttributeUsage(AttributeTargets.Class)]
public class ExampleAttribute : Attribute
{
    public string Example { get; }
    
    public ExampleAttribute(params object[] args)
    {
        Example = string.Join(" ", args.Select(arg => arg));
    }
}
