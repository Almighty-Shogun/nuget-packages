using AlmightyShogun.AspNet.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Validates that a password reset token exists and is active.
/// </summary>
///
/// <param name="serviceProvider">The service provider used to resolve validation dependencies.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class PasswordResetTokenRule(IServiceProvider serviceProvider) : ICustomValidationRule<object, string>
{
    /// <summary>
    /// The authentication validation service used by this rule.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly IAuthValidationService _authValidationService = serviceProvider.GetRequiredService<IAuthValidationService>();

    /// <inheritdoc />
    public async Task<ValidationRuleResult> ValidateAsync(object request, string? value, CancellationToken cancellationToken = default)
        => await IsActiveTokenAsync(value, cancellationToken);

    /// <summary>
    /// Checks whether the supplied reset token is active.
    /// </summary>
    ///
    /// <param name="value">The password reset token value.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ///
    /// <returns>The validation result for the reset token.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private async Task<ValidationRuleResult> IsActiveTokenAsync(string? value, CancellationToken cancellationToken)
        => await _authValidationService.IsPasswordResetTokenActiveAsync(value, cancellationToken)
            ? ValidationRuleResult.Success()
            : ValidationRuleResult.Failure("passwords.token");
}
