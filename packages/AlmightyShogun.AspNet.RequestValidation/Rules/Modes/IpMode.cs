namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Whether either address family is accepted, or only one of them.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal enum IpMode
{
    /// <summary>
    /// Allows either IPv4 or IPv6 addresses.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Any,

    /// <summary>
    /// Requires an IPv4 address.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Ipv4,

    /// <summary>
    /// Requires an IPv6 address.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Ipv6
}
