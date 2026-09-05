---
fields:
    - name: Id
      description: The surrogate key, used for foreign keys inside the package. Never put it in a response; `Identifier` is the value a client is given.
      type: int

    - name: Identifier
      description: The public identifier, a version 7 GUID so rows sort by creation without leaking a sequence. This is what appears in the access token and what the services accept.
      type: Guid

    - name: Username
      description: The account name, uniquely indexed and accepted by login alongside the email address.
      type: string

    - name: Email
      description: The address, uniquely indexed. Also what the forgot-password flow matches against.
      type: string

    - name: Password
      description: The password hash produced by ASP.NET Core's hasher. Rehashed in place on sign-in when the hasher reports an outdated format.
      type: string

    - name: Sessions
      description: The user's refresh-token sessions. Not loaded unless explicitly included.
      type: 'List<UserSession>'
      default: '[]'

    - name: Role
      description: The single role written into the access token as a role claim.
      type: string
      default: User

    - name: Permissions
      description: The permission values written into the token, one claim each. Store `api:users.read` style values only when routes are scoped per application; otherwise store plain values such as `users.read`.
      type: 'string[]'
      default: '[]'

    - name: IsActive
      description: Whether the account may sign in. A false value is refused after the password is checked, so it never reveals that an account exists.
      type: bool
      default: 'true'

    - name: Lockout
      description: The run of failed sign-ins against the account, or null while there is none. Held in its own table and not loaded unless explicitly included.
      type: UserLockout?
      default: 'null'

    - name: TwoFactor
      description: The user's TOTP enrolment, or null when they never began one. Held in its own table and not loaded unless explicitly included.
      type: UserTwoFactor?
      default: 'null'
---

# AuthUser

The base user entity every credential service works against. Applications inherit from it to add their own profile fields, and the derived type becomes the `TUser` of the context and the services.

::: danger
`AuthUser` is a database entity and must not cross the API boundary in either direction. Never return it from an endpoint: it carries the password hash, the surrogate key, and any loaded sessions, so map it to a DTO that exposes only the fields the client needs. Never bind a client payload straight onto it either: `Role` and `Permissions` are ordinary settable properties that become claims in the user's own access token, so build it from a [`RegisterRequest`](../requests/register-request) and set those values in application code.
:::

## Usage

```csharp
using AlmightyShogun.AspNet.Auth.Credentials;

public sealed class AppUser : AuthUser
{
    public string DisplayName { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}
```

<FrontmatterDocs/>
