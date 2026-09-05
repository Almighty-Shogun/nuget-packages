---
params:
    - name: configuration
      description: Configuration carrying the `Email` section. An absent section binds successfully and fails validation instead, which is what reports the missing token rather than a binding error.
      type: IConfiguration

returns: The `IServiceCollection` instance with the Resend client, template loader, and mail service registered.
---

# AddResendEmail

Binds the `Email` section to [`EmailSettings`](../configuration), then registers the shared template loader, [`IResendMailService`](../services/resend-mail-service), and the Resend client itself, authenticated with the configured token and placed behind a typed `HttpClient` with the standard resilience handler. Call it once during startup, then send classes that inherit from [`BaseMailTemplate`](../types/base-mail-template) through the injected service. The mail service is transient and the template loader a singleton, so its file cache is shared across sends, while settings are read through `IOptions<EmailSettings>` and a configuration reload requires a restart.

## Usage

```csharp
using AlmightyShogun.Mail.Resend;

builder.Services.AddResendEmail(builder.Configuration);
```

## Template files

The package ships no templates. It reads `BaseEmailTemplate.html`, `BaseEmailParagraph.html`, and `BaseEmailButton.html` from a `mail` folder under `AppContext.BaseDirectory`, so an application supplies the three itself and copies them to the output folder on publish.

A missing folder or template throws an `InvalidOperationException` naming what to add. That check runs while registering rather than while the host starts, so a test that only builds a service collection hits it too.

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection AddResendEmail(
    IConfiguration configuration
);
```
