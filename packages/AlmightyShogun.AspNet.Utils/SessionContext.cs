namespace AlmightyShogun.AspNet.Utils;

/// <summary>
/// Represents request metadata captured for the current HTTP request.
/// </summary>
///
/// <param name="IpAddress">The captured client IP address.</param>
/// <param name="UserAgent">The captured raw User-Agent header value.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>2.2.1</since>
public record SessionContext(string? IpAddress, string? UserAgent)
{
    /// <summary>
    /// Gets the <c>HttpContext.Items</c> key used to store the current request session context.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.4.0</since>
    public const string ItemKey = nameof(SessionContext);
}
