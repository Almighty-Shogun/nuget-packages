using System.Collections.Immutable;
using System.Net;

namespace AlmightyShogun.AspNet.Core;

/// <summary>
/// Provides Cloudflare defaults for forwarded-header configuration.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public static class CloudflareDefaults
{
    /// <summary>
    /// The header containing the originating client IP address.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public const string ClientIpHeader = "CF-Connecting-IP";

    /// <summary>
    /// The published Cloudflare IPv4 and IPv6 network ranges.
    /// </summary>
    ///
    /// <remarks>
    /// These ranges are stored statically and are not fetched or updated at runtime.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public static ImmutableArray<IPNetwork> Networks { get; } =
    [
        // IPv4
        IPNetwork.Parse("173.245.48.0/20"),
        IPNetwork.Parse("103.21.244.0/22"),
        IPNetwork.Parse("103.22.200.0/22"),
        IPNetwork.Parse("103.31.4.0/22"),
        IPNetwork.Parse("141.101.64.0/18"),
        IPNetwork.Parse("108.162.192.0/18"),
        IPNetwork.Parse("190.93.240.0/20"),
        IPNetwork.Parse("188.114.96.0/20"),
        IPNetwork.Parse("197.234.240.0/22"),
        IPNetwork.Parse("198.41.128.0/17"),
        IPNetwork.Parse("162.158.0.0/15"),
        IPNetwork.Parse("104.16.0.0/13"),
        IPNetwork.Parse("104.24.0.0/14"),
        IPNetwork.Parse("172.64.0.0/13"),
        IPNetwork.Parse("131.0.72.0/22"),

        // IPv6
        IPNetwork.Parse("2400:cb00::/32"),
        IPNetwork.Parse("2606:4700::/32"),
        IPNetwork.Parse("2803:f800::/32"),
        IPNetwork.Parse("2405:b500::/32"),
        IPNetwork.Parse("2405:8100::/32"),
        IPNetwork.Parse("2a06:98c0::/29"),
        IPNetwork.Parse("2c0f:f248::/32")
    ];
}
