# AuthUser

Base credential user entity used by the package services. Application user entities should inherit from `AuthUser` when they need extra profile or domain fields while still using the built-in login, password, session, and token services.

The package requires `Username`, `Email`, `Password`, `Role`, and `Permissions` because those values are used for credential lookup, password verification, JWT role claims, and permission claims. Store plain permissions such as `users.read` for single-host APIs. Store app-scoped permissions with their app prefix, for example `api:users.read`, only when routes use app-prefixed [`AuthPermission`](/asp-net-jwt-auth/attributes/auth-permission-attribute) values for host/app scoped authorization. The `Password` and `Sessions` properties are ignored during JSON serialization.

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
    public required string Username { get; set; }
    public required string Email { get; set; }
    public string Password { get; set; } = string.Empty;
    public List<UserSession> Sessions { get; set; } = [];
    public string Role { get; set; } = "User";
    public string[] Permissions { get; set; } = [];
}
```
