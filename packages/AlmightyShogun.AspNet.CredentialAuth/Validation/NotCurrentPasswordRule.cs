using AlmightyShogun.AspNet.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Validates that a new password does not reuse the current stored password.
/// </summary>
///
/// <param name="serviceProvider">The service provider used to resolve validation dependencies.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class NotCurrentPasswordRule(IServiceProvider serviceProvider) :
    ICustomValidationRule<ChangePasswordRequest, string>,
    ICustomValidationRule<CompleteForgotPasswordRequest, string>
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
        => await _authValidationService.IsDifferentFromCurrentPasswordAsync(value, cancellationToken)
            ? ValidationRuleResult.Success()
            : ValidationRuleResult.Failure("passwords.reused");

    /// <inheritdoc />
    public async Task<ValidationRuleResult> ValidateAsync(
        CompleteForgotPasswordRequest request,
        string? value,
        CancellationToken cancellationToken = default)
        => await _authValidationService
            .IsDifferentFromPasswordResetTokenUserAsync(request.Token, value, cancellationToken)
            ? ValidationRuleResult.Success()
            : ValidationRuleResult.Failure("passwords.reused");
}
