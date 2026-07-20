namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the min dimensions validation rule for a request property.
/// </summary>
///
/// <param name="width">The minimum image width in pixels.</param>
/// <param name="height">The minimum image height in pixels.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class MinDimensionsAttribute(int width, int height)
    : ValidationRuleAttribute(FileConstraintMode.MinDimensions, width, height);
