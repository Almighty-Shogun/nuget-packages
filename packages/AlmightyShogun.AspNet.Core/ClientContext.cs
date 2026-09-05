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
    /// The address the client connected from, or <c>null</c> when none was recorded.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.2.1</since>
    public required string? IpAddress { get; init; }

    /// <summary>
    /// The User-Agent header exactly as sent, unparsed and untrusted, or <c>null</c> when none was recorded. Pass
    /// it to <c>UserAgent.Parse</c> when the browser or device is what matters.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.2.1</since>
    public required string? UserAgent { get; init; }
}
