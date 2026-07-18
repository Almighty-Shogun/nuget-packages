namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Defines example arguments for a console command.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ExampleAttribute : Attribute
{
    /// <summary>
    /// Gets the generated command example argument text.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public string Example { get; }

    /// <summary>
    /// Creates an example from one or more argument values.
    /// </summary>
    ///
    /// <param name="args">Example argument values that are joined with spaces.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public ExampleAttribute(params object[] args)
    {
        Example = string.Join(" ", args.Select(arg => arg));
    }
}
