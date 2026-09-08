namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// The marker the assembly scan and the registration match a command class on, not a service type: implementations are
/// registered under their own concrete type, so nothing is ever resolved as an <see cref="IConsoleCommand"/>. The metadata
/// declared here is implemented explicitly by <see cref="ConsoleCommandBase"/> from the class attributes, which
/// <see cref="ConsoleCommandExtensions"/> and <see cref="ConsoleCommandDiscovery"/> read directly, so naming a command
/// never constructs one.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
internal interface IConsoleCommand
{
    /// <summary>
    /// The primary name the command answers to, taken from the class attribute rather than the class name.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    string Name { get; }

    /// <summary>
    /// The explanation for a help listing, or <c>null</c> when the command was declared without one.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    string? Description { get; }

    /// <summary>
    /// The extra names the command answers to, or an empty list when it declares none.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    IReadOnlyList<string> Aliases { get; }
}
