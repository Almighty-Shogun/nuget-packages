namespace AlmightyShogun.AspNet.Core;

/// <summary>
/// The caller identity behind one request: where it connected from and what client it claims to be. Both values are a
/// snapshot taken when the context is built, so it outlives the request it came from and can be handed to a background
/// job or an audit record.
/// </summary>
///
/// <param name="IpAddress">
/// The client address, already normalized out of its IPv4-mapped IPv6 form. <c>null</c> when the connection has none,
/// as on an in-memory test server.
/// </param>
/// <param name="UserAgent">
/// The User-Agent header exactly as sent, unparsed and untrusted. Blank rather than <c>null</c> when the header was
/// absent; call <c>GetUserAgent</c> when the browser or device is what matters.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>2.2.1</since>
public sealed record SessionContext(string? IpAddress, string? UserAgent)
{
    /// <summary>
    /// The <c>HttpContext.Items</c> key a context is read from. Exposed so a consumer can seed the entry directly, for
    /// a test that needs a fixed address without a real connection behind it, or to capture it once per request.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.4.0</since>
    public const string ItemKey = nameof(SessionContext);
}
