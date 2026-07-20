# LoginIdentifierExistsAttribute

Validates that a login identifier matches an existing credential user by username or email address. [`LoginRequest`](../requests/login-request) uses this attribute on `Identifier` so login validation can return the same `auth.failed` message for an unknown identifier as for an invalid password.

Use this attribute when the request property contains either a username or an email address and should resolve to a stored [`AuthUser`](../types/auth-user). Missing values are left to `[Required]`, so a blank identifier can produce the normal required-field validation error instead of a credential lookup error.

## Usage

```csharp
using AlmightyShogun.AspNet.Validation;
using AlmightyShogun.AspNet.CredentialAuth;

public sealed class LoginRequest
{
    [Required]
    [LoginIdentifierExists]
    public string Identifier { get; set; } = string.Empty;
}
```

## Type signature

```csharp
[AttributeUsage(AttributeTargets.Property)]
public sealed class LoginIdentifierExistsAttribute : CustomRuleAttribute;
```
