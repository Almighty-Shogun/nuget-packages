namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the same as validation rule for a request property.
/// </summary>
///
/// <param name="field">The other request field that must match this field.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class SameAsAttribute(string field) : ValidationRuleAttribute(FieldComparisonMode.Same, field);
