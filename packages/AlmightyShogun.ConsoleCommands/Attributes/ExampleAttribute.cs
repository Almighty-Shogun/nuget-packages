using System.Globalization;

namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Declares example arguments for a console command.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ExampleAttribute : Attribute
{
    /// <summary>
    /// Gets the formatted example arguments as a single space-separated string.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public string Example { get; }

    /// <summary>
    /// Gets the individually formatted example arguments.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal string[] Arguments { get; }

    /// <summary>
    /// Creates an example from the supplied argument values.
    /// </summary>
    ///
    /// <param name="args">
    /// The example argument values.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public ExampleAttribute(params object[] args)
    {
        Arguments =
        [
            .. args.Select(static arg => arg switch
            {
                null => string.Empty,
                IFormattable formattable =>
                    formattable.ToString(null, CultureInfo.InvariantCulture),
                _ => arg.ToString() ?? string.Empty
            })
        ];

        Example = string.Join(" " , Arguments);
    }
}
