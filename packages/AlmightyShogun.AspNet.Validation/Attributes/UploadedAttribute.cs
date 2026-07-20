namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the uploaded validation rule for a request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class UploadedAttribute() : ValidationRuleAttribute(FileConstraintMode.Uploaded);
