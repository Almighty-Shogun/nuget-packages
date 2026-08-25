namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the uploaded image to match the exact width and height. An absent or empty value passes, so pair it with
/// <see cref="RequiredAttribute"/> when the field is mandatory.
/// </summary>
///
/// <param name="width">The required image width in pixels.</param>
/// <param name="height">The required image height in pixels.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class DimensionsAttribute(int width, int height) : ValidationRuleAttribute(FileConstraintMode.Dimensions, width, height);
