namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Marks a class as a console command and carries the metadata the dispatcher needs.
/// Required on every <see cref="ConsoleCommandBase"/> subclass.
/// </summary>
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ConsoleCommandAttribute : Attribute
{
    /// <summary>
    /// Marks a class as a console command and carries the metadata the dispatcher needs.
    /// Required on every <see cref="ConsoleCommandBase"/> subclass.
    /// </summary>
    ///
    /// <param name="name">The first token typed at the prompt, matched case-insensitively.</param>
    /// <param name="description">
    /// The one-line explanation, surfaced as <see cref="ConsoleCommand.Description"/> for an application to render. Left
    /// <c>null</c> when omitted; nothing in this package prints it.
    /// </param>
    /// <param name="ignoreExtraArgs">
    /// When <c>true</c>, the command still runs when more tokens are typed than its handler declares parameters.
    /// </param>
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
    /// The name the command is invoked by, before any alias is considered.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public string Name { get; }

    /// <summary>
    /// The explanation for a help listing, or <c>null</c> when the command was declared without one.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public string? Description { get; }

    /// <summary>
    /// Whether the command tolerates more typed tokens than its handler declares parameters.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public bool IgnoreExtraArgs { get; }
    
    public ArgumentParsingMode ArgumentParsing { get; set; }
        = ArgumentParsingMode.Spaces;
}
