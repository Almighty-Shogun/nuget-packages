namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the between validation rule for a request property.
/// </summary>
///
/// <param name="min">The inclusive minimum value.</param>
/// <param name="max">The inclusive maximum value.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class BetweenAttribute(double min, double max) : ValidationRuleAttribute(ComparableSizeMode.Between, min, max);
