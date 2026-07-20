# AuthUser

Base credential user entity used by the package services. Application user entities should inherit from `AuthUser` when they need extra profile or domain fields while still using the built-in login, password, session, and token services.

The package requires `Username`, `Email`, `Password`, `Role`, and `Permissions` because those values are used for credential lookup, password verification, JWT role claims, and permission claims. The `Password` and `Sessions` properties are ignored during JSON serialization.

## Usage

```csharp
using AlmightyShogun.AspNet.CredentialAuth;

public sealed class AppUser : AuthUser
{
    public string DisplayName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
```

## Type signature

```csharp
[Table("users")]
public class AuthUser
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public List<UserSession> Sessions { get; set; }
    public string Role { get; set; }
    public string[] Permissions { get; set; }
}
```
