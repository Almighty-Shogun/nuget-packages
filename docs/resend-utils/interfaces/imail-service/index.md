# IMailService

Mail sending contract registered by `AddResendEmail`. Application services should depend on this interface when they need to send a `BaseMailTemplate` through the configured Resend integration.

The package provides the implementation internally. This keeps application code focused on the stable public contract while the package owns the Resend client, template loading, rendering, error logging, and sender configuration details.

## Usage

```csharp
using AlmightyShogun.Resend.Utils;

public sealed class AccountMailer(IMailService mailService)
{
    public Task<bool> SendWelcomeEmailAsync(string recipientEmail)
    {
        WelcomeMailTemplate template = new("Akari");

        return mailService.SendAsync(recipientEmail, template);
    }
}
```

## Methods

- [SendAsync](./send-async) &mdash; sends a rendered mail template to a recipient.
