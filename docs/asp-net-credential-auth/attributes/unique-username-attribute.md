# UniqueUsernameAttribute

Validates that a username is not already assigned to another credential user. [`RegisterRequest`](../requests/register-request) and [`CreateUserRequest`](../requests/create-user-request) use this attribute so user creation can reject duplicate usernames before the database unique index is hit.

Use this attribute on username fields that should be unique in the credential user table. Missing values are left to `[Required]`, so the uniqueness rule does not replace required-field validation.

## Usage

```csharp
using AlmightyShogun.AspNet.Validation;
using AlmightyShogun.AspNet.CredentialAuth;

public sealed class RegisterRequest
{
    [Required]
    [UniqueUsername]
    public string Username { get; set; } = string.Empty;
}
```

## Type signature

```csharp
[AttributeUsage(AttributeTargets.Property)]
public sealed class UniqueUsernameAttribute : CustomRuleAttribute;
```
