namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Exposes the dispatch metadata of a command without requiring an instance to be built for it. It is the marker the
/// assembly scan looks for, not a service type: registration adds each command under its concrete type, and the handler
/// fills its name table from the <see cref="ConsoleCommandDescriptor"/> singletons registered alongside them.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
internal interface IConsoleCommand
{
    /// <summary>
    /// Gets the primary name the command answers to, taken from the class attribute rather than the class name.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    string Name { get; }

    /// <summary>
    /// Gets the explanation for a help listing, or <c>null</c> when the command was declared without one.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    string? Description { get; }

    /// <summary>
    /// Gets the extra names the command answers to, or an empty list when it declares none.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    IReadOnlyList<string> Aliases { get; }
}
