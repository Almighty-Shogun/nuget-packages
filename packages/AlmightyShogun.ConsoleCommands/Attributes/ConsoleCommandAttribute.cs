namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Marks a class as a console command and defines its metadata.
/// </summary>
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ConsoleCommandAttribute : Attribute
{
    /// <summary>
    /// Creates a console command declaration.
    /// </summary>
    ///
    /// <param name="name">The primary command name.</param>
    /// <param name="description">
    /// The command description, if any
    /// </param>
    /// <param name="ignoreExtraArgs">
    /// Whether arguments beyond the declared handler parameters are ignored.
    /// </param>
    ///
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="name"/> is blank or contains whitespace.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public ConsoleCommandAttribute(string name, string? description = null, bool ignoreExtraArgs = false)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (name.Any(char.IsWhiteSpace))
            throw new ArgumentException("Command name must not contain whitespace.", nameof(name));

        Name = name;
        Description = description;
        IgnoreExtraArgs = ignoreExtraArgs;
    }

    /// <summary>
    /// Gets the primary command name.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public string Name { get; }

    /// <summary>
    /// Gets the command description, if any.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public string? Description { get; }

    /// <summary>
    /// Gets whether arguments beyond the declared handler parameters are ignored.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public bool IgnoreExtraArgs { get; }
    
    /// <summary>
    /// Gets or sets how command arguments are parsed.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public ArgumentParsingMode ArgumentParsing { get; set; }
        = ArgumentParsingMode.Spaces;
}
