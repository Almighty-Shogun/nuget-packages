namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the mime types validation rule for a request property.
/// </summary>
///
/// <param name="mimeTypes">The allowed MIME types.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class MimeTypesAttribute(params string[] mimeTypes)
    : ValidationRuleAttribute(FileConstraintMode.MimeTypes, mimeTypes);
