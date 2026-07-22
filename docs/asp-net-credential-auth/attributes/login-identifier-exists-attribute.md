# LoginIdentifierExistsAttribute

Validates that a login identifier matches an existing credential user by username or email address. [`LoginRequest`](../requests/login-request) uses this attribute on `Identifier` so unknown identifiers are rejected during request validation before the login service creates a session.

Use this attribute when the request property contains either a username or an email address and should resolve to a stored [`AuthUser`](../types/auth-user). Missing values are left to `[Required]`, so a blank identifier can produce the normal required-field validation error instead of a credential lookup error. [`CurrentPassword`](./current-password-attribute) validates the matching password for login requests.

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
