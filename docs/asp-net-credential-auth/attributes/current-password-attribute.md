# CurrentPasswordAttribute

Validates a password against credential data stored by Credential Auth. On [`ChangePasswordRequest`](../requests/change-password-request), it checks the submitted current password against the currently authenticated user. On [`LoginRequest`](../requests/login-request), it checks the submitted password against the user found by the request `Identifier`.

Use this attribute through the package request DTOs unless the backing custom rule has been extended for a custom request type. When the password does not match, the rule returns `passwords.current` for password changes or `auth.failed` for login attempts.

## Usage

::: code-group

```csharp [LoginRequest.cs]
using AlmightyShogun.AspNet.Validation;
using AlmightyShogun.AspNet.CredentialAuth;

public sealed class LoginRequest
{
    [Required]
    [LoginIdentifierExists]
    public string Identifier { get; set; } = string.Empty;

    [Required]
    [CurrentPassword]
    public string Password { get; set; } = string.Empty;
}
```

```csharp [ChangePasswordRequest.cs]
using AlmightyShogun.AspNet.Validation;
using AlmightyShogun.AspNet.CredentialAuth;

public sealed class ChangePasswordRequest
{
    [Required]
    [CurrentPassword]
    public string CurrentPassword { get; set; } = string.Empty;
}
```

:::

## Type signature

```csharp
[AttributeUsage(AttributeTargets.Property)]
public sealed class CurrentPasswordAttribute : CustomRuleAttribute;
```
