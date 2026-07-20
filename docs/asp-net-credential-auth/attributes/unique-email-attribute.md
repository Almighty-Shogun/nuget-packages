# UniqueEmailAttribute

Validates that an email address is not already assigned to another credential user. [`CreateUserRequest`](../requests/create-user-request) uses this attribute so registration can reject duplicate email addresses before the database unique index is hit.

Use this attribute on string email fields that should be unique in the credential user table. Missing values are left to `[Required]`, and invalid email formatting should still be handled by `[Email]` from [ASP.NET Validation](/asp-net-validation/validation-rules/formats).

## Usage

```csharp
using AlmightyShogun.AspNet.Validation;
using AlmightyShogun.AspNet.CredentialAuth;

public sealed class CreateUserRequest
{
    [Email]
    [Required]
    [UniqueEmail]
    public string Email { get; set; } = string.Empty;
}
```

## Type signature

```csharp
[AttributeUsage(AttributeTargets.Property)]
public sealed class UniqueEmailAttribute : CustomRuleAttribute;
```
