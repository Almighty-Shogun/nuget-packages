# Mail Resend

Adds Resend email sending with dependency injection, strongly typed email settings, and reusable mail templates. The package is meant for applications that want a consistent HTML and plain-text email layout without building the Resend message object every time an email is sent.

Application code creates classes inheriting [`BaseMailTemplate`](./types/base-mail-template) and sends them through [`IResendMailService`](./services/resend-mail-service), so every email shares the same brand values, footer, logo, and button rendering.

## Categories

- [Configuration](./configuration) &mdash; the `Email` section and the settings records bound from it.
- [Extensions](./extensions/add-resend-email) &mdash; startup extension methods for registering Resend email services.
- [Services](./services/resend-mail-service) &mdash; dependency-injection mail sending services.
- [Types](./types/base-mail-template) &mdash; mail template base types used by application code.
- [Records](./records/mail-options) &mdash; small data records used by mail templates.

## Quick Example

::: code-group

```csharp [Program.cs]
using AlmightyShogun.Mail.Resend;

builder.Services.AddResendEmail(builder.Configuration);
```

```csharp [WelcomeMailTemplate.cs]
using AlmightyShogun.Mail.Resend;

public sealed class WelcomeMailTemplate(string name) : BaseMailTemplate
{
    public override string Subject => "Welcome to Shogun";
    protected override string Title => "Welcome";
    protected override string Greeting => $"Hello {name},";

    protected override IReadOnlyList<string> Paragraphs =>
    [
        "Your account is ready to use."
    ];
}
```

```csharp [SignupService.cs]
using AlmightyShogun.Mail.Resend;

public sealed class SignupService(IResendMailService mailService)
{
    public Task SendWelcomeAsync(string email, string name)
        => mailService.SendAsync(email, new WelcomeMailTemplate(name));
}
```

:::
