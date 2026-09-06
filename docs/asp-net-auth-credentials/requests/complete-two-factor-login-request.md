---
fields:
    - name: Challenge
      description: The challenge [`LoginAsync`](../services/auth-user-service#loginasync) handed back, submitted in the form it was returned rather than decoded or trimmed. It stands for a password that already verified, so post it in the body and keep it out of URLs and logs.
      type: string

    - name: Code
      description: The code from the authenticator app, or one of the recovery codes issued at enrolment. Both are tried, so nothing here says which was sent.
      type: string
---

# CompleteTwoFactorLoginRequest

What [`CompleteTwoFactorLoginAsync`](../services/auth-user-service#completetwofactorloginasync) takes to finish a sign-in the second factor stopped. Validation only checks that both values are present; whether the challenge is still redeemable and whether the code verifies are decided by the service.

<FrontmatterDocs/>

## Type signature

```csharp
public sealed record CompleteTwoFactorLoginRequest
{
    public required string Challenge { get; set; }
    public required string Code { get; set; }
}
```
