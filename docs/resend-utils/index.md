# Resend Utils

Adds Resend email sending with dependency injection, strongly typed email settings, and reusable mail templates. The package is meant for applications that want a consistent HTML and plain-text email layout without building the Resend message object every time an email is sent.

Use this package when an application sends transactional or notification emails through Resend and wants those emails to share the same brand values, footer text, logo, and button rendering. Application code normally creates classes that inherit from [`BaseMailTemplate`](./types/base-mail-template), injects [`IResendMailService`](./services/resend-mail-service), and sends those templates through the registered mail service.

## Categories

- [Configuration](./configuration/email-settings) &mdash; public configuration records bound from application configuration.
- [Extensions](./extensions/add-resend-email) &mdash; startup extension methods for registering Resend email services.
- [Services](./services/resend-mail-service) &mdash; dependency-injection mail sending services.
- [Types](./types/base-mail-template) &mdash; mail template base types used by application code.
- [Records](./records/mail-button) &mdash; small data records used by mail templates.

## Quick Example

```csharp
using AlmightyShogun.Resend.Utils;

builder.Services.AddResendEmail(builder.Configuration);
```

Create a template by inheriting from [`BaseMailTemplate`](./types/base-mail-template), then inject [`IResendMailService`](./services/resend-mail-service) where the application needs to send it.

```csharp
using AlmightyShogun.Resend.Utils;

public sealed class WelcomeMailTemplate(string displayName) : BaseMailTemplate
{
    public override string Subject => "Welcome to Shogun";

    protected override string Title => "Welcome";

    protected override string Greeting => $"Hello {displayName},";

    protected override IReadOnlyList<string> Paragraphs =>
    [
        "Your account is ready to use."
    ];
}
```
