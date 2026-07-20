namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the IP address validation rule for a request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class IpAttribute() : ValidationRuleAttribute(IpMode.Any);
