namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Marks a class as a remote command and defines the name clients use to address it.
/// </summary>
///
/// <param name="name">The command name used on the wire.</param>
/// <param name="description">An optional description of the command.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class RemoteCommandAttribute(string name, string? description = null) : Attribute
{
    /// <summary>
    /// Gets the command name used on the wire.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public string Name { get; } = name;

    /// <summary>
    /// Gets the optional command description.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public string? Description { get; } = description;
}
