namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Allows only files with one of the provided file extensions. An absent or empty value passes, so pair it with
/// <see cref="RequiredAttribute"/> when the field is mandatory.
/// </summary>
///
/// <param name="extensions">The allowed file extensions.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ExtensionsAttribute(params string[] extensions) : ValidationRuleAttribute(FileConstraintMode.Extensions, extensions);
