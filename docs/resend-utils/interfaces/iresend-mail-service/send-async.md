---
outline: deep

params:
    - name: recipientEmail
      description: Email address that should receive the rendered message.
      type: string

    - name: mail
      description: Mail template instance that supplies the subject, body text, and buttons.
      type: BaseMailTemplate

returns: '`true` when the email was sent successfully; otherwise `false`.'
---

# SendAsync

Sends a `BaseMailTemplate` to a single recipient through the configured Resend integration. The package renders the template as HTML and plain text, applies the configured sender and brand settings, sends the message through Resend, and returns whether the send completed successfully.

The method returns `false` when the API token or sender email is blank, or when the Resend send operation fails. Callers can use that boolean to decide whether to retry, show an error state, or record application-specific delivery status.

## Usage

::: code-group

```csharp [InvoiceMailer.cs]
using AlmightyShogun.Resend.Utils;

public sealed class InvoiceMailer(IResendMailService mailService)
{
    public async Task<bool> SendInvoiceReadyAsync(string recipientEmail, string invoiceUrl)
    {
        InvoiceReadyMailTemplate template = new(invoiceUrl);

        return await mailService.SendAsync(recipientEmail, template);
    }
}
```

```csharp [InvoiceReadyMailTemplate.cs]
using AlmightyShogun.Resend.Utils;

public sealed class InvoiceReadyMailTemplate(string invoiceUrl) : BaseMailTemplate
{
    public override string Subject => "Your invoice is ready";

    protected override string Title => "Invoice ready";

    protected override string Greeting => "Hello,";

    protected override IReadOnlyList<string> Paragraphs =>
    [
        "Your invoice has been generated and is ready to view."
    ];

    protected override IReadOnlyList<MailButton> Buttons =>
    [
        new("View invoice", invoiceUrl)
    ];
}
```

:::

<FrontmatterDocs/>

## Type signature

```csharp
public Task<bool> SendAsync(
    string recipientEmail,
    BaseMailTemplate mail
);
```

## Uses

- [BaseMailTemplate](../../classes/base-mail-template/)
- [EmailSettings](../../configuration/email-settings)
