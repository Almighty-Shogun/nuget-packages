namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Defines one or more aliases that invoke the same console command.
/// </summary>
///
/// <param name="aliases">Aliases that can be typed instead of the command name.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
[AttributeUsage(AttributeTargets.Class)]
public sealed class AliasAttribute(params string[] aliases) : Attribute
{
    /// <summary>
    /// Gets aliases that can be typed instead of the command name.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public string[] Aliases { get; } = aliases;
}
