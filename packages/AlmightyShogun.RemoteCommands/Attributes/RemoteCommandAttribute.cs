namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Marks a class as a remote command and declares the name clients address it by. Required on every
/// <see cref="RemoteCommand{T}"/> subclass: a class without it stops registration before the host is built, and one
/// constructed directly throws from the base constructor instead.
/// </summary>
///
/// <param name="name">
/// The wire name, matched with ordinal case sensitivity against the request's <c>command</c> field. A name already
/// claimed by another command is dropped with a warning, leaving that command unreachable.
/// </param>
/// <param name="description">Documentation for whoever writes the client. Nothing on the wire reads it.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class RemoteCommandAttribute(string name, string? description = null) : Attribute
{
    /// <summary>
    /// Gets the wire name, which is a contract with every client already sending it and so is not safe to rename.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public string Name { get; } = name;

    /// <summary>
    /// Gets the description, or <c>null</c> when the command was declared without one.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public string? Description { get; } = description;
}
