---
fields:
    - name: Username
      description: The account name to claim. Refused with `UsernameTakenException` when another account already holds it.
      type: string

    - name: Email
      description: The address to claim, checked for a valid shape by `[Email]`. Refused with `EmailTakenException` when another account already holds it.
      type: string

    - name: Password
      description: The initial password, at least 8 characters and subject to the `[PasswordSecure]` rule. Hashed before the row is written and never stored as given.
      type: string
---

# RegisterRequest

The three values a user may supply about themselves when signing up. It carries no role or permission field on purpose, since anything a client can send there ends up as claims in its own access token.

<FrontmatterDocs/>

## Type signature

```csharp
public sealed record RegisterRequest
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}
```
