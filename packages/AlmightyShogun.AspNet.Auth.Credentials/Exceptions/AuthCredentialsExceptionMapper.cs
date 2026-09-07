using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Core;

namespace AlmightyShogun.AspNet.Auth.Credentials;

/// <summary>
/// Maps this package's exceptions to the responses they produce, so the exceptions themselves stay plain. Internal, and
/// both registered and consumed as this concrete type, so nothing outside the package can resolve it or map any of these
/// exceptions differently.
/// </summary>
///
/// <remarks>
/// Registered by <c>AddAuthCredentials</c> whether or not it also registers the handler chain.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal sealed class AuthCredentialsExceptionMapper : IExceptionMapper
{
    /// <inheritdoc />
    public ErrorMapping? Map(Exception exception) => exception switch
    {
        PasswordMismatchException => new ErrorMapping
        {
            StatusCode = StatusCodes.Status422UnprocessableEntity,
            Code = "password_mismatch",
            MessageKey = "passwords.mismatch",
            MessageParameters = []
        },

        PasswordReusedException => new ErrorMapping
        {
            StatusCode = StatusCodes.Status422UnprocessableEntity,
            Code = "password_reused",
            MessageKey = "passwords.reused",
            MessageParameters = []
        },

        UsernameTakenException => new ErrorMapping
        {
            StatusCode = StatusCodes.Status422UnprocessableEntity,
            Code = "username_taken",
            MessageKey = "auth.username-taken",
            MessageParameters = []
        },

        EmailTakenException => new ErrorMapping
        {
            StatusCode = StatusCodes.Status422UnprocessableEntity,
            Code = "email_taken",
            MessageKey = "auth.email-taken",
            MessageParameters = []
        },

        InvalidCredentialsException => new ErrorMapping
        {
            StatusCode = StatusCodes.Status401Unauthorized,
            Code = "invalid_credentials",
            MessageKey = "auth.failed",
            MessageParameters = []
        },

        InvalidSessionException => new ErrorMapping
        {
            StatusCode = StatusCodes.Status401Unauthorized,
            Code = "invalid_session",
            MessageKey = "auth.session-invalid",
            MessageParameters = []
        },

        InvalidTwoFactorCodeException => new ErrorMapping
        {
            StatusCode = StatusCodes.Status401Unauthorized,
            Code = "invalid_two_factor_code",
            MessageKey = "auth.two-factor-invalid",
            MessageParameters = []
        },

        InvalidTwoFactorChallengeException => new ErrorMapping
        {
            StatusCode = StatusCodes.Status401Unauthorized,
            Code = "invalid_two_factor_challenge",
            MessageKey = "auth.two-factor-challenge-invalid",
            MessageParameters = []
        },

        AccountDisabledException => new ErrorMapping
        {
            StatusCode = StatusCodes.Status403Forbidden,
            Code = "account_disabled",
            MessageKey = "auth.disabled",
            MessageParameters = []
        },

        ConcurrentSessionUpdateException => new ErrorMapping
        {
            StatusCode = StatusCodes.Status409Conflict,
            Code = "concurrent_session_update",
            MessageKey = "auth.session-update-conflict",
            MessageParameters = []
        },

        InvalidPasswordResetTokenException => new ErrorMapping
        {
            StatusCode = StatusCodes.Status410Gone,
            Code = "invalid_password_reset_token",
            MessageKey = "passwords.token-invalid",
            MessageParameters = []
        },

        InvalidEmailVerificationTokenException => new ErrorMapping
        {
            StatusCode = StatusCodes.Status410Gone,
            Code = "invalid_email_verification_token",
            MessageKey = "auth.email-verification-token-invalid",
            MessageParameters = []
        },

        AccountLockedException lockedOutException => new ErrorMapping
        {
            StatusCode = StatusCodes.Status423Locked,
            Code = "account_locked_out",
            MessageKey = "auth.locked-out",
            MessageParameters = [lockedOutException.LockoutEnd]
        },

        _ => null
    };
}
