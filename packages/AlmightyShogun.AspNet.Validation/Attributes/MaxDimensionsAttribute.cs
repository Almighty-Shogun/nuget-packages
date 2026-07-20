namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the max dimensions validation rule for a request property.
/// </summary>
///
/// <param name="width">The maximum image width in pixels.</param>
/// <param name="height">The maximum image height in pixels.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class MaxDimensionsAttribute(int width, int height)
    : ValidationRuleAttribute(FileConstraintMode.MaxDimensions, width, height);
