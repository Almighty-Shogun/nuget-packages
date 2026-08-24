---
fields:
    - name: To
      description: Recipients. Must not be empty; a send with no recipient fails without calling Resend.
      type: 'IReadOnlyList<string>'

    - name: Cc
      description: Carbon-copy recipients.
      type: 'IReadOnlyList<string>'
      default: '[]'

    - name: Bcc
      description: Blind carbon-copy recipients.
      type: 'IReadOnlyList<string>'
      default: '[]'

    - name: ReplyTo
      description: Addresses a reply should go to instead of the configured sender.
      type: 'IReadOnlyList<string>'
      default: '[]'

    - name: Attachments
      description: Files delivered with the message, as [`MailAttachment`](./mail-attachment) values. Each one is held in memory for the duration of the send.
      type: 'IReadOnlyList<MailAttachment>'
      default: '[]'

    - name: IdempotencyKey
      description: Key Resend uses to collapse duplicate sends. One is generated per send when this is null, so the resilience handler's retries cannot deliver twice. Set it when the whole operation is itself retryable, such as a background job.
      type: string?
      default: 'null'
---

# MailOptions

Everything about a send other than the message itself: recipients, copies, reply-to, attachments, and the idempotency key.

An options object rather than a growing parameter list, so adding a capability does not change [`SendAsync`](../services/resend-mail-service#sendasync). The single-recipient overload builds one of these for you.

## Usage

```csharp
using AlmightyShogun.Mail.Resend;

MailSendResult result = await mailService.SendAsync(
    new WelcomeMail(user.Name),
    new MailOptions
    {
        To = [user.Email],
        Bcc = ["audit@example.com"],
        ReplyTo = ["support@example.com"]
    },
    cancellationToken
);
```

<FrontmatterDocs/>

## Type signature

```csharp
public sealed record MailOptions
{
    public required IReadOnlyList<string> To { get; init; }
    public IReadOnlyList<string> Cc { get; init; }
    public IReadOnlyList<string> Bcc { get; init; }
    public IReadOnlyList<string> ReplyTo { get; init; }
    public IReadOnlyList<MailAttachment> Attachments { get; init; }
    public string? IdempotencyKey { get; init; }
}
```
