namespace AlmightyShogun.RemoteCommands;

/// <summary>
/// Marks a class as a remote command and defines the command metadata read by the listener.
/// </summary>
///
/// <param name="name">The command name expected in incoming payloads.</param>
/// <param name="description">An optional command description.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
[AttributeUsage(AttributeTargets.Class)]
public sealed class RemoteCommandAttribute(string name, string? description = null) : Attribute
{
    /// <summary>
    /// Gets the command name expected in incoming payloads.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public string Name { get; init; } = name;

    /// <summary>
    /// Gets the optional command description.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public string? Description { get; } = description;
}
