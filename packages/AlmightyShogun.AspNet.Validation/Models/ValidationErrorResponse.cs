using AlmightyShogun.AspNet.Utils;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Represents the standardized validation error response body.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed record ValidationErrorResponse : HttpErrorResponse
{
    /// <summary>
    /// Gets the validation errors grouped by request field.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required IReadOnlyDictionary<string, ValidationRuleError> Errors { get; init; }
}
