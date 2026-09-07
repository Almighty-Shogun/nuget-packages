---
fields:
    - name: Token
      description: The token from the verification email, submitted in the form it was sent rather than decoded or trimmed. It identifies the user on its own, so nothing else about the account is submitted with it.
      type: string
---

# CompleteEmailVerificationRequest

What [`CompleteVerificationAsync`](../services/auth-email-service#completeverificationasync) and [`CompleteEmailChangeAsync`](../services/auth-email-service#completeemailchangeasync) both take to redeem a token. The endpoint receiving it decides which flow is being completed, so the record carries no field naming one and a link cannot be spent on the wrong path. The name refers to the [`EmailVerificationToken`](../types/email-verification-token) being redeemed rather than to the outcome, which is why it reads as a verification on the change path too.

<FrontmatterDocs/>

## Type signature

```csharp
public sealed record CompleteEmailVerificationRequest
{
    public string Token { get; set; } = "";
}
```
