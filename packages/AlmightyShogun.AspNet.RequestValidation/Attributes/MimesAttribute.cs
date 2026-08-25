namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Allows only files matching the provided MIME extension aliases. An absent or empty value passes, so pair it with
/// <see cref="RequiredAttribute"/> when the field is mandatory.
/// </summary>
///
/// <param name="mimes">The allowed file extensions or MIME aliases.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class MimesAttribute(params string[] mimes) : ValidationRuleAttribute(FileConstraintMode.Mimes, mimes);
