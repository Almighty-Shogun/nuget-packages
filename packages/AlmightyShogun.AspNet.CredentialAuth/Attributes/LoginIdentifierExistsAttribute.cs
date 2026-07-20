using AlmightyShogun.AspNet.Validation;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Declares that the login identifier must match an existing user.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class LoginIdentifierExistsAttribute : CustomRuleAttribute
{
    /// <inheritdoc />
    protected override Type CreateCustomRule() => CustomRule<LoginIdentifierExistsRule>();
}
