# PasswordMatchAttribute

Validates that a confirmation password matches the request `NewPassword` value. The package uses this attribute on [`ChangePasswordRequest`](../requests/change-password-request) and [`CompleteForgotPasswordRequest`](../requests/complete-forgot-password-request) so both password-change and password-reset flows reject mismatched confirmation values with the `passwords.match` message key.

Use this attribute through the package request DTOs unless the backing custom rule has been extended for a custom request type. Missing values are left to `[Required]`, so the match rule does not replace required-field validation.

## Usage

```csharp
using AlmightyShogun.AspNet.Validation;
using AlmightyShogun.AspNet.CredentialAuth;

public sealed class CompleteForgotPasswordRequest
{
    [Required]
    public string NewPassword { get; set; } = string.Empty;

    [Required]
    [PasswordMatch]
    public string ConfirmPassword { get; set; } = string.Empty;
}
```

## Type signature

```csharp
[AttributeUsage(AttributeTargets.Property)]
public sealed class PasswordMatchAttribute : CustomRuleAttribute;
```
