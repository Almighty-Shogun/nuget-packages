# PasswordResetTokenAttribute

Validates that a password reset token exists, has not been used, and has not expired. [`ForgotPasswordTokenRequest`](../requests/forgot-password-token-request) and [`CompleteForgotPasswordRequest`](../requests/complete-forgot-password-request) use this attribute to reject stale or unknown reset tokens before password reset logic runs.

The stored token is never compared in plain text. Credential Auth hashes the submitted token with the same SHA-256 token hashing logic used when the token was created, then checks the hash against active [`PasswordResetToken`](../types/password-reset-token) records.

## Usage

```csharp
using AlmightyShogun.AspNet.Validation;
using AlmightyShogun.AspNet.CredentialAuth;

public sealed class ForgotPasswordTokenRequest
{
    [Required]
    [PasswordResetToken]
    public string Token { get; set; } = string.Empty;
}
```

## Type signature

```csharp
[AttributeUsage(AttributeTargets.Property)]
public sealed class PasswordResetTokenAttribute : CustomRuleAttribute;
```
