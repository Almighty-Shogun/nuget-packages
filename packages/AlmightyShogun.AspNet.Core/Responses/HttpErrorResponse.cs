namespace AlmightyShogun.AspNet.Core;

/// <summary>
/// Represents a standardized HTTP error response.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public record HttpErrorResponse
{
    /// <summary>
    /// The HTTP status code.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public required int Code { get; init; }

    /// <summary>
    /// The machine-readable error code.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public required string Error { get; init; }

    /// <summary>
    /// The human-readable error description, or <c>null</c> when unavailable.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public string? ErrorDescription { get; init; }
}
