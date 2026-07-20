using AlmightyShogun.AspNet.Validation;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Declares that the property must match the request password field.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class PasswordMatchAttribute : CustomRuleAttribute
{
    /// <inheritdoc />
    protected override Type CreateCustomRule() => CustomRule<PasswordMatchRule>();
}
