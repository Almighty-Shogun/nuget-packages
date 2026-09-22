using AlmightyShogun.AspNet.Localization;

namespace AlmightyShogun.AspNet.Core;

/// <summary>
/// Describes how an exception maps to an HTTP error response.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public sealed record ErrorMapping
{
    /// <summary>
    /// The HTTP status code.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public required int StatusCode { get; init; }

    /// <summary>
    /// The machine-readable error code.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public required string Code { get; init; }

    /// <summary>
    /// The message key used to resolve the human-readable description.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public required string MessageKey { get; init; }

    /// <summary>
    /// The parameters used to format the resolved message.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public required IReadOnlyList<object?> MessageParameters { get; init; }
}
