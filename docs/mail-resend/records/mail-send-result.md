---
fields:
    - name: IsSuccess
      description: Whether Resend accepted the message.
      type: bool
    - name: MessageId
      description: Resend's identifier for the accepted message. Use it to correlate a send with a webhook or the Resend dashboard.
      type: string?
    - name: Error
      description: The failure description when the send did not succeed.
      type: string?
---

# MailSendResult

The outcome of a send. Returned rather than thrown, so a caller can decide whether a failed notification should fail the operation around it. Only the package constructs one, so a result always describes an outcome that actually happened.

## Usage

```csharp
using AlmightyShogun.Mail.Resend;

WelcomeMail mail = new WelcomeMail(user.Name);
MailSendResult result = await mailService.SendAsync(user.Email, mail);

if (!result.IsSuccess)
{
    logger.LogWarning("Welcome mail failed: {Error}", result.Error);
}
```

<FrontmatterDocs/>

## Type signature

```csharp
public sealed record MailSendResult
{
    public bool IsSuccess { get; }
    public string? MessageId { get; }
    public string? Error { get; }
}
```
