using AlmightyShogun.AspNet.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Validates that a username is not already used by another authentication user.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class UniqueUsernameRule(IServiceProvider serviceProvider) : ICustomValidationRule<object, string>
{
    /// <summary>
    /// The authentication validation service used by this rule.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly IAuthValidationService _authValidationService = serviceProvider
        .GetRequiredService<IAuthValidationService>();

    /// <inheritdoc />
    public async Task<ValidationRuleResult> ValidateAsync(
        object request,
        string? value,
        CancellationToken cancellationToken = default)
    {
        return await _authValidationService.IsUsernameUniqueAsync(value, cancellationToken)
            ? ValidationRuleResult.Success()
            : ValidationRuleResult.Failure("validation.unique");
    }
}
