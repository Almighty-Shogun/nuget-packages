---
fields:
    - name: Email
      description: The address to send a reset link to, checked for a valid shape by `[Email]`. An address matching no account is not an error, so the response cannot be used to test which addresses are registered.
      type: string
---

# ForgotPasswordRequest

What [`RequestForgotPasswordAsync`](../services/auth-password-service#requestforgotpasswordasync) takes to start a reset. It returns the token in plain text for the application to email, or `null` when nothing matched.

<FrontmatterDocs/>

## Type signature

```csharp
public sealed record ForgotPasswordRequest
{
    public required string Email { get; set; }
}
```
