namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the extensions validation rule for a request property.
/// </summary>
///
/// <param name="extensions">The allowed file extensions.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ExtensionsAttribute(params string[] extensions)
    : ValidationRuleAttribute(FileConstraintMode.Extensions, extensions);
