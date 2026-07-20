# NotCurrentPasswordAttribute

Validates that a new password does not match the current stored password for the affected user. On [`ChangePasswordRequest`](../requests/change-password-request), the rule compares `NewPassword` with the currently authenticated user's stored password hash. On [`CompleteForgotPasswordRequest`](../requests/complete-forgot-password-request), it resolves the active reset token first and compares the new password with that token owner's stored password hash.

Place the attribute on `NewPassword`. [`PasswordMatch`](./password-match-attribute) then makes `ConfirmPassword` follow the same value, so both submitted password fields are prevented from reusing the current password. Missing values are left to `[Required]`, and invalid reset tokens are left to [`PasswordResetToken`](./password-reset-token-attribute).

## Usage

::: code-group

```csharp [ChangePasswordRequest.cs]
using AlmightyShogun.AspNet.Validation;
using AlmightyShogun.AspNet.CredentialAuth;

public sealed class ChangePasswordRequest
{
    [Required]
    [CurrentPassword]
    public required string CurrentPassword { get; set; }

    [Min(8)]
    [Required]
    [PasswordSecure]
    [NotCurrentPassword]
    public required string NewPassword { get; set; }

    [Min(8)]
    [Required]
    [PasswordMatch]
    public required string ConfirmPassword { get; set; }
}
```

```csharp [CompleteForgotPasswordRequest.cs]
using AlmightyShogun.AspNet.Validation;
using AlmightyShogun.AspNet.CredentialAuth;

public sealed class CompleteForgotPasswordRequest
{
    [Required]
    [PasswordResetToken]
    public required string Token { get; set; }

    [Min(8)]
    [Required]
    [PasswordSecure]
    [NotCurrentPassword]
    public required string NewPassword { get; set; }

    [Min(8)]
    [Required]
    [PasswordMatch]
    public required string ConfirmPassword { get; set; }
}
```

:::

## Type signature

```csharp
[AttributeUsage(AttributeTargets.Property)]
public sealed class NotCurrentPasswordAttribute : CustomRuleAttribute;
```
