using AlmightyShogun.AspNet.Validation;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Validates that a password confirmation matches the requested password.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class PasswordMatchRule :
    ICustomValidationRule<ChangePasswordRequest, string>,
    ICustomValidationRule<CompleteForgotPasswordRequest, string>
{
    /// <inheritdoc />
    public Task<ValidationRuleResult> ValidateAsync(
        ChangePasswordRequest request,
        string? value,
        CancellationToken cancellationToken = default)
        => Task.FromResult(PasswordMatches(request.NewPassword, value)
            ? ValidationRuleResult.Success()
            : ValidationRuleResult.Failure("passwords.match"));

    /// <inheritdoc />
    public Task<ValidationRuleResult> ValidateAsync(
        CompleteForgotPasswordRequest request,
        string? value,
        CancellationToken cancellationToken = default)
        => Task.FromResult(PasswordMatches(request.NewPassword, value)
            ? ValidationRuleResult.Success()
            : ValidationRuleResult.Failure("passwords.match"));

    /// <summary>
    /// Checks whether a request password field matches a confirmation value.
    /// </summary>
    ///
    /// <param name="password">The password value.</param>
    /// <param name="confirmPassword">The confirmation password value.</param>
    ///
    /// <returns><c>true</c> when the values match or either value is missing; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool PasswordMatches(string? password, string? confirmPassword)
        => password is null || confirmPassword is null || string.Equals(password, confirmPassword, StringComparison.Ordinal);
}
