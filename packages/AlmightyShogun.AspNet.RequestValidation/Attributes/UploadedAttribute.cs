namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Requires the uploaded file to be present and non-empty. An absent or empty value passes, so pair it with <see cref="RequiredAttribute"/>
/// when the field is mandatory.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class UploadedAttribute() : ValidationRuleAttribute(FileConstraintMode.Uploaded);
