namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the image validation rule for a request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ImageAttribute() : ValidationRuleAttribute(FileConstraintMode.Image);
