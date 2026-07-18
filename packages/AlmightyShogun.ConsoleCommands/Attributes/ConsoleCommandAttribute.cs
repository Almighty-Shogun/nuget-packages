namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Marks a class as a console command and defines the runtime command metadata.
/// </summary>
///
/// <param name="name">The command name typed as the first console input token.</param>
/// <param name="description">An optional command description used by command metadata.</param>
/// <param name="ignoreExtraArgs">Whether unexpected trailing arguments should be ignored.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
[AttributeUsage(AttributeTargets.Class)]
public sealed class ConsoleCommandAttribute(string name, string? description = null, bool ignoreExtraArgs = false) : Attribute
{
    /// <summary>
    /// Gets the command name typed as the first console input token.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public string Name { get; } = name;

    /// <summary>
    /// Gets the optional command description used by command metadata.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public string? Description { get; } = description;

    /// <summary>
    /// Gets whether unexpected trailing arguments should be ignored.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public bool IgnoreExtraArgs { get; } = ignoreExtraArgs;
}
