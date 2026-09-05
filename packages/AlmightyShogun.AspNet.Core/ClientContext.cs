namespace AlmightyShogun.AspNet.Core;

/// <summary>
/// What one request says about the client behind it: where it connected from and what client it claims to be. Both
/// values are a snapshot taken when the context is built, so it outlives the request it came from and can be handed to
/// a background job or an audit record.
/// </summary>
///
/// <remarks>
/// Neither value identifies a caller. An address is shared by everyone behind a proxy and a User-Agent is whatever the
/// client typed, so this belongs in a log or an audit trail rather than in an authorization decision.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>2.2.1</since>
public sealed record ClientContext
{
    /// <summary>
    /// Gets the client address, normalized out of its IPv4-mapped IPv6 form when built by <c>GetClientContext</c>,
    /// though a context constructed directly may hold either. <c>null</c> when the connection has none, as on an
    /// in-memory test server.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.2.1</since>
    public required string? IpAddress { get; init; }

    /// <summary>
    /// Gets the User-Agent header exactly as sent, unparsed and untrusted. Blank rather than <c>null</c> for a request
    /// that carried no header, though a context constructed directly may hold either; pass it to <c>UserAgent.Parse</c>
    /// when the browser or device is what matters.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.2.1</since>
    public required string? UserAgent { get; init; }
}
