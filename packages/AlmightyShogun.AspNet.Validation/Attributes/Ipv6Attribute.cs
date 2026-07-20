namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the IPv6 address validation rule for a request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class Ipv6Attribute() : ValidationRuleAttribute(IpMode.Ipv6);
