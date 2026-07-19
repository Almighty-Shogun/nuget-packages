namespace AlmightyShogun.AspNet.Utils;

/// <summary>
/// Represents the standardized HTTP error response body.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public record HttpErrorResponse
{
    /// <summary>
    /// Gets the HTTP status code returned by the response.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required int Code { get; init; }

    /// <summary>
    /// Gets the stable error identifier for the status code.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required string Error { get; init; }

    /// <summary>
    /// Gets the resolved error description or fallback message key.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required string? ErrorDescription { get; init; }
}
