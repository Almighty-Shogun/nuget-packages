using AlmightyShogun.AspNet.Validation;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Declares that the property must contain a unique username.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
[AttributeUsage(AttributeTargets.Property)]
public sealed class UniqueUsernameAttribute : CustomRuleAttribute
{
    /// <inheritdoc />
    protected override Type CreateCustomRule() => CustomRule<UniqueUsernameRule>();
}
