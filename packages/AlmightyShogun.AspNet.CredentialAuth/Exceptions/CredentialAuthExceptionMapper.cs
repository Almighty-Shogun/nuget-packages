using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Core;

namespace AlmightyShogun.AspNet.CredentialAuth;

/// <summary>
/// Maps this package's exceptions to the responses they produce, so the exceptions themselves stay plain and an
/// application can override any of them by registering its own mapper afterwards.
/// </summary>
///
/// <remarks>
/// Registered by <c>AddCredentialAuth</c> whether or not it also registers the handler chain, since a mapper nothing
/// consults is inert and an application that runs its own handler still wants these mappings available to it.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class CredentialAuthExceptionMapper : IExceptionMapper
{
    /// <inheritdoc />
    public ErrorMapping? Map(Exception exception) => exception switch
    {
        PasswordMismatchException => new ErrorMapping(
            StatusCodes.Status422UnprocessableEntity,
            "password_mismatch",
            "passwords.mismatch",
            []
        ),

        PasswordReusedException => new ErrorMapping(
            StatusCodes.Status422UnprocessableEntity,
            "password_reused",
            "passwords.reused",
            []
        ),

        UsernameTakenException => new ErrorMapping(
            StatusCodes.Status422UnprocessableEntity,
            "username_taken",
            "auth.username-taken",
            []
        ),

        EmailTakenException => new ErrorMapping(
            StatusCodes.Status422UnprocessableEntity,
            "email_taken",
            "auth.email-taken",
            []
        ),

        InvalidCredentialsException => new ErrorMapping(
            StatusCodes.Status401Unauthorized,
            "invalid_credentials",
            "auth.failed",
            []
        ),

        InvalidSessionException => new ErrorMapping(
            StatusCodes.Status401Unauthorized,
            "invalid_session",
            "auth.session-invalid",
            []
        ),

        InvalidTwoFactorCodeException => new ErrorMapping(
            StatusCodes.Status401Unauthorized,
            "invalid_two_factor_code",
            "auth.two-factor-invalid",
            []
        ),

        AccountDisabledException => new ErrorMapping(
            StatusCodes.Status403Forbidden,
            "account_disabled",
            "auth.disabled",
            []
        ),

        InvalidPasswordResetTokenException => new ErrorMapping(
            StatusCodes.Status410Gone,
            "invalid_password_reset_token",
            "passwords.token-invalid",
            []
        ),

        AccountLockedException lockedOutException => new ErrorMapping(
            StatusCodes.Status423Locked,
            "account_locked_out",
            "auth.locked-out",
            [lockedOutException.LockoutEnd]
        ),

        _ => null
    };
}
