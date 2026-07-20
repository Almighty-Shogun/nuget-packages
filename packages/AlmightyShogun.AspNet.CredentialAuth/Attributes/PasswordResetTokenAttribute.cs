using AlmightyShogun.AspNet.Validation;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Declares that the property must contain an active password reset token.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class PasswordResetTokenAttribute : CustomRuleAttribute
{
    /// <inheritdoc />
    protected override Type CreateCustomRule() => CustomRule<PasswordResetTokenRule>();
}
