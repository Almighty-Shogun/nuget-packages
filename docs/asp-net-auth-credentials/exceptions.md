# Exceptions

Every exception the package throws is a plain exception carrying no message of its own. A mapper built on [`IExceptionMapper`](/asp-net-core/exceptions) turns each into a status code, a machine-readable `error` value, and a message key resolved through [Localization](./localization).

| Exception | Status | `error` |
|---|---|---|
| `InvalidCredentialsException` | `401` | `invalid_credentials` |
| `InvalidSessionException` | `401` | `invalid_session` |
| `InvalidTwoFactorCodeException` | `401` | `invalid_two_factor_code` |
| `InvalidTwoFactorChallengeException` | `401` | `invalid_two_factor_challenge` |
| `AccountDisabledException` | `403` | `account_disabled` |
| `ConcurrentSessionUpdateException` | `409` | `concurrent_session_update` |
| `InvalidPasswordResetTokenException` | `410` | `invalid_password_reset_token` |
| `InvalidEmailVerificationTokenException` | `410` | `invalid_email_verification_token` |
| `PasswordMismatchException` | `422` | `password_mismatch` |
| `PasswordReusedException` | `422` | `password_reused` |
| `UsernameTakenException` | `422` | `username_taken` |
| `EmailTakenException` | `422` | `email_taken` |
| `AccountLockedException` | `423` | `account_locked_out` |

::: tip
Pass `registerExceptionHandler: false` to [`AddAuthCredentials`](./extensions/add-auth-credentials) to answer these with a handler of your own. The mapper that produces the statuses above is internal, so a replacement maps these exceptions itself rather than reusing it.
:::

## InvalidCredentialsException

Thrown for **every** credential failure: an unknown identifier, a wrong password, or a wrong current password during a password change.

It is **deliberately one exception rather than several**. Reporting "no such user" separately from "wrong password" tells an attacker which addresses are registered, so do not catch it and re-throw something more specific.

### Type signature

```csharp
public sealed class InvalidCredentialsException : Exception;
```

## AccountLockedException

Thrown when the lockout policy has locked the account, on login, on [`VerifyAsync`](./services/auth-two-factor-service#verifyasync) and the [`CompleteTwoFactorLoginAsync`](./services/auth-user-service#completetwofactorloginasync) that calls it, and on session refresh. Carries `LockoutEnd`, which is passed to the message as `{0}` so the text can say when to try again.

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

Thrown when a refresh token matches no usable session, whether it is unknown, expired, revoked, or scoped to a different application, and when the session it names has reached the [`AbsoluteSessionLifetimeDays`](./configuration) ceiling measured from when it was created.

### Type signature

```csharp
public sealed class InvalidSessionException : Exception;
```

## ConcurrentSessionUpdateException

Thrown by [`ChangePasswordAsync`](./services/auth-password-service#changepasswordasync), [`CompleteForgotPasswordAsync`](./services/auth-password-service#completeforgotpasswordasync), [`CompleteVerificationAsync`](./services/auth-email-service#completeverificationasync), and [`CompleteEmailChangeAsync`](./services/auth-email-service#completeemailchangeasync) when every attempt at the operation lost a race with a concurrent write to the same rows. The three that revoke sessions in the same transaction that changes the credential can lose to a refresh on another of the user's devices; all four write the user's own row, so another sign-in rehashing that password is enough to defeat any of them.

Nothing was written: each attempt runs in a transaction of its own and none of them reached its commit, so the credential still stands, a reset token or verification link is still unspent, and the same request can be sent again. The database failure the last attempt lost to is kept as the inner exception, which is `null` when that attempt lost to a guarded update matching no row rather than to a failed statement.

### Type signature

```csharp
public sealed class ConcurrentSessionUpdateException(
    Exception? innerException = null
) : Exception;
```

## InvalidPasswordResetTokenException

Thrown when a reset token is unknown, already used, or expired. `410` rather than `404`, because the resource existed and is gone.

### Type signature

```csharp
public sealed class InvalidPasswordResetTokenException : Exception;
```

## InvalidEmailVerificationTokenException

Thrown when a verification token is unknown, already spent, expired, issued for the other purpose, or a registration token naming an address the account no longer holds. `410` rather than `404`, because the resource existed and is gone.

All five causes answer identically, so the endpoint cannot be used to learn which tokens once existed, which flow one belongs to, or what address the account is on. A token a concurrent request spends first answers as an already spent one, since redemption claims the row with a guarded update rather than on the strength of the read that found it.

### Type signature

```csharp
public sealed class InvalidEmailVerificationTokenException : Exception;
```

## InvalidTwoFactorCodeException

Thrown when completing enrolment with a wrong code, when a user with no enrolment at all is asked to verify one, and by [`CompleteTwoFactorLoginAsync`](./services/auth-user-service#completetwofactorloginasync) for a code the verification refused, replayed codes and spent recovery codes included. [`VerifyAsync`](./services/auth-two-factor-service#verifyasync) itself returns `false` for those rather than throwing.

The challenge is left unspent, so a mistyped code can be corrected without sending the password again.

### Type signature

```csharp
public sealed class InvalidTwoFactorCodeException : Exception;
```

## InvalidTwoFactorChallengeException

Thrown by [`CompleteTwoFactorLoginAsync`](./services/auth-user-service#completetwofactorloginasync) when the challenge cannot be redeemed, whether it is unknown, already spent, past its expiry, or issued through a different application than the one completing it.

All four answer identically, so the endpoint cannot be used to learn which challenges once existed. A challenge another request claims first answers the same way, since completion claims the row with a guarded update rather than on the strength of the read that found it.

### Type signature

```csharp
public sealed class InvalidTwoFactorChallengeException : Exception;
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

Also thrown by [`RequestEmailChangeAsync`](./services/auth-email-service#requestemailchangeasync) and again by [`CompleteEmailChangeAsync`](./services/auth-email-service#completeemailchangeasync), since the check made when the change was requested cannot hold the address until it is redeemed.

### Type signature

```csharp
public sealed class EmailTakenException : Exception;
```
