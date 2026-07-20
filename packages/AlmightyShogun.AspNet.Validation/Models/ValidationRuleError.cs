namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Represents a single validation rule error in the response body.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed record ValidationRuleError
{
    /// <summary>
    /// Gets the numeric validation error code.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required long Code { get; init; }

    /// <summary>
    /// Gets the validation error identifier.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required string Error { get; init; }

    /// <summary>
    /// Gets the resolved validation error description.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required string? ErrorDescription { get; init; }
}
