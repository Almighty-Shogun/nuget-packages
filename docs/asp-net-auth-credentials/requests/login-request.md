---
fields:
    - name: Identifier
      description: A username or an email address. Both are matched, so one login form serves users who remember either.
      type: string

    - name: Password
      description: The password as typed. It is verified against the stored hash by the service, not during request validation.
      type: string
---

# LoginRequest

The credentials [`LoginAsync`](../services/auth-user-service#loginasync) takes. Validation only checks that both values are present; whether they are correct is decided by the service, so a wrong password and an unknown user come back identically.

<FrontmatterDocs/>

## Type signature

```csharp
public sealed record LoginRequest
{
    public required string Identifier { get; set; }
    public required string Password { get; set; }
}
```
