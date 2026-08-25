namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// The metadata describing one discovered command, built by reflection for a help listing to render. It is a read model
/// and runs nothing, so holding one never constructs the command class it came from.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
public sealed class ConsoleCommand
{
    /// <summary>
    /// Assembles the metadata, prefixing the command name onto the usage and example text so both read as something the
    /// user could type. Internal because the values only mean anything when reflected off a real command class.
    /// </summary>
    ///
    /// <param name="name">The name the command is invoked by.</param>
    /// <param name="description">The explanation for a listing, or <c>null</c> when the command declares none.</param>
    /// <param name="aliases">The extra names the command answers to, empty when it declares none.</param>
    /// <param name="usage">The parameter shape, without the command name. Blank for a command that takes no arguments.</param>
    /// <param name="example">The sample argument values, without the command name, or <c>null</c> when none were declared.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    internal ConsoleCommand(string name, string? description, IReadOnlyList<string> aliases, string usage, string? example)
    {
        Name = name;
        Description = description;
        Aliases = aliases;
        Usage = string.IsNullOrWhiteSpace(usage) ? name : $"{name} {usage}";
        Example = string.IsNullOrWhiteSpace(example) ? null : $"{name} {example}";
    }

    /// <summary>
    /// Gets the name the command is invoked by, which is also what the usage and example text are prefixed with.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public string Name { get; }

    /// <summary>
    /// Gets the explanation for a listing, or <c>null</c> when the command was declared without one.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public string? Description { get; }

    /// <summary>
    /// Gets the extra names the command answers to, or an empty list when it declares none.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public IReadOnlyList<string> Aliases { get; }

    /// <summary>
    /// Gets the full line to type, as the name followed by one <c>&lt;name:Type&gt;</c> placeholder per handler parameter.
    /// A command taking no arguments yields the bare name, never a trailing space.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public string Usage { get; }

    /// <summary>
    /// Gets a complete sample invocation including the command name, or <c>null</c> when the command declares no example.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public string? Example { get; }
}
