namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the field value to be different from another request field.
/// </summary>
///
/// <param name="field">The other request field that must have a different value.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class DifferentAttribute(string field) : ValidationRuleAttribute(FieldComparisonMode.Different, field);
