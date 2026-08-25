namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the uploaded image to be at least the provided width and height. An absent or empty value passes, so pair it with
/// <see cref="RequiredAttribute"/> when the field is mandatory.
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
