using AlmightyShogun.AspNet.Validation;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Declares that the property must not match the current stored password.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class NotCurrentPasswordAttribute : CustomRuleAttribute
{
    /// <inheritdoc />
    protected override Type CreateCustomRule() => CustomRule<NotCurrentPasswordRule>();
}
