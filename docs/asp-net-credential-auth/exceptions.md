# Exceptions

Every exception the package throws is a plain exception carrying no message of its own. A mapper built on [`IExceptionMapper`](/asp-net-core/exceptions) turns each into a status code, a machine-readable `error` value, and a message key resolved through [Localization](./localization).

| Exception | Status | `error` |
|---|---|---|
| `InvalidCredentialsException` | `401` | `invalid_credentials` |
| `InvalidSessionException` | `401` | `invalid_session` |
| `InvalidTwoFactorCodeException` | `401` | `invalid_two_factor_code` |
| `AccountDisabledException` | `403` | `account_disabled` |
| `InvalidPasswordResetTokenException` | `410` | `invalid_password_reset_token` |
| `PasswordMismatchException` | `422` | `password_mismatch` |
| `PasswordReusedException` | `422` | `password_reused` |
| `UsernameTakenException` | `422` | `username_taken` |
| `EmailTakenException` | `422` | `email_taken` |
| `AccountLockedException` | `423` | `account_locked_out` |

## Usage

```csharp
using AlmightyShogun.AspNet.CredentialAuth;

try
{
    AuthSessionResult<AppUser> result = await authUserService
        .LoginAsync(request, HttpContext);
}
catch (AccountLockedException exception)
{
    logger.LogInformation(
        "Login blocked until {LockoutEnd}",
        exception.LockoutEnd
    );

    throw;
}
```

::: tip
Pass `registerExceptionHandler: false` to [`AddCredentialAuth`](./extensions/add-credential-auth) to answer these with a handler of your own. The mapper stays registered either way, so a replacement can resolve it and reuse the table above.
:::

## InvalidCredentialsException

Thrown for **every** credential failure: an unknown identifier, a wrong password, or a wrong current password during a password change.

It is **deliberately one exception rather than several**. Reporting "no such user" separately from "wrong password" tells an attacker which addresses are registered, so do not catch it and re-throw something more specific.

### Type signature

```csharp
public sealed class InvalidCredentialsException : Exception;
```

## AccountLockedException

Thrown when the lockout policy has locked the account, both on login and on session refresh. Carries `LockoutEnd`, which is passed to the message as `{0}` so the text can say when to try again.

Only thrown when [lockout](./configuration) is enabled.

### Type signature

```csharp
public sealed class AccountLockedException(
    DateTimeOffset lockoutEnd
) : Exception;
```

## AccountDisabledException

Thrown when credentials are correct but `IsActive` is `false`, and when refreshing a session for a disabled account.

Checked **after** the password, so it does not confirm an account exists to someone who does not know the password.

### Type signature

```csharp
public sealed class AccountDisabledException : Exception;
```

## InvalidSessionException

Thrown when a refresh token matches no usable session, whether it is unknown, expired, revoked, or scoped to a different application.

### Type signature

```csharp
public sealed class InvalidSessionException : Exception;
```

## InvalidPasswordResetTokenException

Thrown when a reset token is unknown, already used, or expired. `410` rather than `404`, because the resource existed and is gone.

### Type signature

```csharp
public sealed class InvalidPasswordResetTokenException : Exception;
```

## InvalidTwoFactorCodeException

Thrown when completing enrolment with a wrong code, and when a user with no enrolment at all is asked to verify one. [`VerifyAsync`](./services/auth-two-factor-service#verifyasync) returns `false` instead of throwing, because a wrong code during sign-in is an ordinary outcome.

### Type signature

```csharp
public sealed class InvalidTwoFactorCodeException : Exception;
```

## PasswordMismatchException

Thrown when the new password and its confirmation differ, on both [`ChangePasswordAsync`](./services/auth-password-service#changepasswordasync) and [`CompleteForgotPasswordAsync`](./services/auth-password-service#completeforgotpasswordasync).

### Type signature

```csharp
public sealed class PasswordMismatchException : Exception;
```

## PasswordReusedException

Thrown when the new password verifies against the password already stored, so a change that changes nothing is refused rather than silently accepted.

### Type signature

```csharp
public sealed class PasswordReusedException : Exception;
```

## UsernameTakenException

Thrown by [`CreateUserAsync`](./services/auth-user-service#createuserasync) and [`RegisterAsync`](./services/auth-user-service#registerasync) when another account already holds the username, compared under the database's collation.

### Type signature

```csharp
public sealed class UsernameTakenException : Exception;
```

## EmailTakenException

Thrown by [`CreateUserAsync`](./services/auth-user-service#createuserasync) and [`RegisterAsync`](./services/auth-user-service#registerasync) when another account already holds the email address, compared under the database's collation.

### Type signature

```csharp
public sealed class EmailTakenException : Exception;
```
