using AlmightyShogun.AspNet.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Validates that a supplied password matches the relevant credential user.
/// </summary>
///
/// <param name="serviceProvider">The service provider used to resolve validation dependencies.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class CurrentPasswordRule(IServiceProvider serviceProvider) :
    ICustomValidationRule<ChangePasswordRequest, string>,
    ICustomValidationRule<LoginRequest, string>
{
    /// <summary>
    /// The authentication validation service used by this rule.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly IAuthValidationService _authValidationService = serviceProvider.GetRequiredService<IAuthValidationService>();

    /// <inheritdoc />
    public async Task<ValidationRuleResult> ValidateAsync(
        ChangePasswordRequest request,
        string? value,
        CancellationToken cancellationToken = default)
        => await _authValidationService.IsCurrentPasswordAsync(value, cancellationToken)
            ? ValidationRuleResult.Success()
            : ValidationRuleResult.Failure("passwords.current");

    /// <inheritdoc />
    public async Task<ValidationRuleResult> ValidateAsync(
        LoginRequest request,
        string? value,
        CancellationToken cancellationToken = default)
        => await _authValidationService.IsCurrentPasswordAsync(request.Identifier, value, cancellationToken)
            ? ValidationRuleResult.Success()
            : ValidationRuleResult.Failure("auth.failed");
}
