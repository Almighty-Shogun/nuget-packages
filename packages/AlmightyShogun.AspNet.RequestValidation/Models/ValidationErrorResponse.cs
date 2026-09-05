using AlmightyShogun.AspNet.Core;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// The body a failed validation returns. It extends the shared error shape, so a client parses one envelope for every kind of failure.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed record ValidationErrorResponse : HttpErrorResponse
{
    /// <summary>
    /// The failures, keyed by field, with one entry per field that failed rather than one per rule that broke.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required IReadOnlyDictionary<string, ValidationRuleError> Errors { get; init; }
}
