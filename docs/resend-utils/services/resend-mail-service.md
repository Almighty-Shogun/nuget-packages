# ResendMailService

Dependency-injection service for sending rendered mail templates through Resend. [`AddResendEmail`](../extensions/add-resend-email) registers the package sender for `IResendMailService`, and application services should depend on the interface when they need to send a [`BaseMailTemplate`](../types/base-mail-template).

The package provides the implementation internally. This keeps application code focused on the stable public contract while the package owns the Resend client, template loading, rendering, error logging, and sender configuration details.

## Usage

::: code-group

```csharp [AccountMailer.cs]
using AlmightyShogun.Resend.Utils;

public sealed class AccountMailer(IResendMailService mailService)
{
    public Task<bool> SendWelcomeEmailAsync(string recipientEmail)
    {
        WelcomeMailTemplate template = new("Shogun");

        return mailService.SendAsync(recipientEmail, template);
    }
}
```

```csharp [WelcomeMailTemplate.cs]
using AlmightyShogun.Resend.Utils;

public sealed class WelcomeMailTemplate(string displayName) : BaseMailTemplate
{
    public override string Subject => "Welcome";

    protected override string Title => "Welcome";

    protected override string Greeting => $"Hello {displayName},";

    protected override IReadOnlyList<string> Paragraphs =>
    [
        "Your account is ready to use."
    ];
}
```

:::

## SendAsync

Sends a [`BaseMailTemplate`](../types/base-mail-template) to a single recipient through the configured Resend integration. The package renders the template as HTML and plain text, applies the configured sender and brand settings, sends the message through Resend, and returns whether the send completed successfully.

The method returns `false` when the API token or sender email is blank, or when the Resend send operation fails. Callers can use that boolean to decide whether to retry, show an error state, or record application-specific delivery status.

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

### Type signature

```csharp
public Task<bool> SendAsync(
    string recipientEmail,
    BaseMailTemplate mail
);
```
