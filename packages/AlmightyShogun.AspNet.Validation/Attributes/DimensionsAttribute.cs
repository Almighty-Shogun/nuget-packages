namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the dimensions validation rule for a request property.
/// </summary>
///
/// <param name="width">The required image width in pixels.</param>
/// <param name="height">The required image height in pixels.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class DimensionsAttribute(int width, int height)
    : ValidationRuleAttribute(FileConstraintMode.Dimensions, width, height);
