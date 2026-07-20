# CurrentPasswordAttribute

Validates a password against credential data stored by Credential Auth. On [`ChangePasswordRequest`](../requests/change-password-request), the attribute checks the value against the currently authenticated user. On [`LoginRequest`](../requests/login-request), it checks the password against the user found by the request `Identifier`.

Use this attribute through the package request DTOs unless the backing custom rule has been extended for a custom request type. When the password does not match, the rule returns `passwords.current` for password changes or `auth.failed` for login attempts.

## Usage

```csharp
using AlmightyShogun.AspNet.CredentialAuth;

public sealed class ChangePasswordRequest
{
    [CurrentPassword]
    public string CurrentPassword { get; set; } = string.Empty;
}
```

## Type signature

```csharp
[AttributeUsage(AttributeTargets.Property)]
public sealed class CurrentPasswordAttribute : CustomRuleAttribute;
```
