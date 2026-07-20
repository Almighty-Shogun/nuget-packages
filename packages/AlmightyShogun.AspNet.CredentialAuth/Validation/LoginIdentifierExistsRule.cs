using AlmightyShogun.AspNet.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Validates that a login identifier matches an existing user.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class LoginIdentifierExistsRule(IServiceProvider serviceProvider) : ICustomValidationRule<object, string>
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
        return await _authValidationService.LoginIdentifierExistsAsync(value, cancellationToken)
            ? ValidationRuleResult.Success()
            : ValidationRuleResult.Failure("auth.failed");
    }
}
