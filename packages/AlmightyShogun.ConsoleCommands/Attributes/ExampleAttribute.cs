namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Declares a sample invocation for a command, so a help listing can show what real arguments look like instead of only
/// the generated parameter shape. Purely descriptive: nothing validates the values against the handler parameters.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ExampleAttribute : Attribute
{
    /// <summary>
    /// Gets the argument values as one space-separated string. The command name is not part of it; the metadata builder
    /// prefixes that when it assembles <see cref="ConsoleCommand.Example"/>.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public string Example { get; }

    /// <summary>
    /// Creates an example from the given values, joining them with single spaces in the order written.
    /// </summary>
    ///
    /// <param name="args">
    /// The argument values, converted with each one's own <c>ToString</c>. Supply them in handler parameter order, since
    /// nothing reorders or names them.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public ExampleAttribute(params object[] args) => Example = string.Join(" ", args);
}
