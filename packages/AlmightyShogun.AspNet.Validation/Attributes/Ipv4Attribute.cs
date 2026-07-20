namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the IPv4 address validation rule for a request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class Ipv4Attribute() : ValidationRuleAttribute(IpMode.Ipv4);
