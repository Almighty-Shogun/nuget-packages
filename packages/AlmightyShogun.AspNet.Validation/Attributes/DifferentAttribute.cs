namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the different validation rule for a request property.
/// </summary>
///
/// <param name="field">The other request field that must have a different value.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class DifferentAttribute(string field)
    : ValidationRuleAttribute(FieldComparisonMode.Different, field);
