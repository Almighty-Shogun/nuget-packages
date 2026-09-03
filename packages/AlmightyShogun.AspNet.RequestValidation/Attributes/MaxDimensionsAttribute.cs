using System.Reflection;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the uploaded image to be no larger than the provided width and height. An absent or empty value passes, so pair it with
/// <see cref="RequiredAttribute"/> when the field is mandatory.
/// </summary>
///
/// <param name="width">The maximum image width in pixels.</param>
/// <param name="height">The maximum image height in pixels.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class MaxDimensionsAttribute(int width, int height) : ValidationRuleAttribute
{
    /// <inheritdoc />
    internal override IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property)
        => new FileConstraintValidationRule<TRequest, TProperty>(
            FileConstraintMode.MaxDimensions,
            dimensionConstraints: new ImageDimensionConstraints(width, height)
        );
}
