namespace AlmightyShogun.AspNet.Core;

/// <summary>
/// Represents client information captured from a request.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.2.1</since>
public sealed record ClientContext
{
    /// <summary>
    /// The client IP address, or <c>null</c> when unavailable.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.2.1</since>
    public required string? IpAddress { get; init; }

    /// <summary>
    /// The raw User-Agent header, or <c>null</c> when unavailable.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.2.1</since>
    public required string? UserAgent { get; init; }
}
