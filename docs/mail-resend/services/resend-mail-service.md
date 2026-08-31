# ResendMailService

Renders a [`BaseMailTemplate`](../types/base-mail-template) and sends it through Resend. Application code depends on `IResendMailService`, which loads the shared template files, renders the HTML and plain-text bodies, and calls the Resend API.

## Usage

::: code-group

```csharp [AccountMailer.cs]
using AlmightyShogun.Mail.Resend;

public sealed class AccountMailer(IResendMailService mailService)
{
    public async Task<string?> SendWelcomeEmailAsync(
        string recipientEmail,
        CancellationToken cancellationToken
    )
    {
        WelcomeMailTemplate template = new("Shogun");

        MailSendResult result = await mailService.SendAsync(
            recipientEmail,
            template,
            cancellationToken
        );

        return result.IsSuccess ? result.MessageId : null;
    }
}
```

```csharp [WelcomeMailTemplate.cs]
using AlmightyShogun.Mail.Resend;

public sealed class WelcomeMailTemplate(string name) : BaseMailTemplate
{
    public override string Subject => "Welcome";

    protected override string Title => "Welcome";

    protected override string Greeting => $"Hello {name},";

    protected override IReadOnlyList<string> Paragraphs =>
    [
        "Your account is ready to use."
    ];
}
```

:::

::: warning
A provider or transport failure comes back as a failed [`MailSendResult`](../records/mail-send-result), but rendering happens before the request and is not guarded. A missing or unreadable template file throws instead, so inspecting the result alone does not catch both.
:::

## SendAsync

Sends a template to one recipient. Use it for the common case that needs no copies, attachments, or reply-to address.

The returned [`MailSendResult`](../records/mail-send-result) reports whether Resend accepted the message, and carries the message id for correlating with a webhook or the dashboard. Acceptance is not delivery. An empty recipient list and a rejection from Resend both come back as a failed result rather than an exception, so a caller sending in a loop does not have to wrap each message. Cancelling the token is the exception to that and propagates, so a caller shutting down is never told the send was rejected.

Every call to this overload generates its own idempotency key. Reach for the [`MailOptions`](../records/mail-options) overload when the caller can itself be retried and the key has to be stable across those retries.

### Type signature

```csharp
public Task<MailSendResult> SendAsync(
    string recipientEmail,
    BaseMailTemplate mail,
    CancellationToken cancellationToken = default
);
```

## SendAsync with options

Sends a template with full addressing: carbon copies, blind copies, reply-to addresses, attachments, and an explicit idempotency key.

::: code-group

```csharp [InvoiceMailer.cs]
using AlmightyShogun.Mail.Resend;

public sealed class InvoiceMailer(IResendMailService mailService)
{
    public async Task<MailSendResult> SendInvoiceReadyAsync(
        string recipientEmail,
        string invoiceUrl,
        byte[] invoicePdf,
        Guid invoiceId,
        CancellationToken cancellationToken
    )
    {
        InvoiceReadyMailTemplate template = new(invoiceUrl);

        MailOptions options = new()
        {
            To = [recipientEmail],
            Bcc = ["billing@example.com"],
            ReplyTo = ["support@example.com"],
            Attachments = [
                new MailAttachment(
                    "invoice.pdf",
                    invoicePdf,
                    "application/pdf"
                )
            ],
            IdempotencyKey = $"invoice-ready-{invoiceId}"
        };

        return await mailService.SendAsync(
            template,
            options,
            cancellationToken
        );
    }
}
```

```csharp [InvoiceReadyMailTemplate.cs]
using AlmightyShogun.Mail.Resend;

public sealed class InvoiceReadyMailTemplate(string url) : BaseMailTemplate
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
        new("View invoice", url)
    ];
}
```

:::

::: tip
Setting `IdempotencyKey` from something the caller already owns, such as the invoice id above, makes the whole operation idempotent rather than only the HTTP call. A retried job then cannot deliver the same message twice.
:::

### Type signature

```csharp
public Task<MailSendResult> SendAsync(
    BaseMailTemplate mail,
    MailOptions options,
    CancellationToken cancellationToken = default
);
```

## PreviewAsync

Renders a template without sending it, returning the HTML body and its plain-text alternative as a [`MailPreview`](../records/mail-preview). Nothing is sent and Resend is never contacted, so a preview works with an API token that a send would reject.

Use it to inspect what a template produces, or to assert on the rendered output in a test.

```csharp
using AlmightyShogun.Mail.Resend;

public sealed class MailPreviewEndpoint(IResendMailService mailService)
{
    public async Task<string> RenderWelcomeAsync(
        CancellationToken cancellationToken
    )
    {
        WelcomeMailTemplate mail = new("Shogun");

        MailPreview preview = await mailService
            .PreviewAsync(mail, cancellationToken);

        return preview.Html;
    }
}
```

### Type signature

```csharp
public Task<MailPreview> PreviewAsync(
    BaseMailTemplate mail,
    CancellationToken cancellationToken = default
);
```
