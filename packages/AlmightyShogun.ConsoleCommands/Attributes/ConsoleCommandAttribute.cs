namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Marks a class as a console command and carries the metadata the dispatcher needs. Required on every
/// <see cref="ConsoleCommandBase"/> subclass: a class without it stops registration before the host is built, and one
/// constructed directly throws from the base constructor instead.
/// </summary>
///
/// <param name="name">
/// The first token typed at the prompt, matched case-insensitively. A name already taken by another command is dropped
/// with a warning, so the losing command stays reachable only through whatever aliases it declares.
/// </param>
/// <param name="description">The one-line explanation shown in a help listing. Omitted commands simply list no text.</param>
/// <param name="ignoreExtraArgs">
/// When <c>true</c>, arguments beyond the handler's parameters are discarded and the command still runs; when
/// <c>false</c>, the extra input is treated as a mistake, logged, and the command is not invoked.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ConsoleCommandAttribute(string name, string? description = null, bool ignoreExtraArgs = false) : Attribute
{
    /// <summary>
    /// Gets the name the command is invoked by, before any alias is considered.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public string Name { get; } = name;

    /// <summary>
    /// Gets the explanation for a help listing, or <c>null</c> when the command was declared without one.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public string? Description { get; } = description;

    /// <summary>
    /// Gets whether surplus arguments are tolerated. It only relaxes the upper bound: a command still refuses to run when
    /// too few arguments are supplied to fill its required parameters.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public bool IgnoreExtraArgs { get; } = ignoreExtraArgs;
}
