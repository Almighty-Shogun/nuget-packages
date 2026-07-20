namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Declares the MIME alias validation rule for a request property.
/// </summary>
///
/// <param name="mimes">The allowed file extensions or MIME aliases.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class MimesAttribute(params string[] mimes) : ValidationRuleAttribute(FileConstraintMode.Mimes, mimes);
