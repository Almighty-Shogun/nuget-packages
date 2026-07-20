namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the mac address validation rule for a request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class MacAddressAttribute() : ValidationRuleAttribute(FormatMode.MacAddress);
