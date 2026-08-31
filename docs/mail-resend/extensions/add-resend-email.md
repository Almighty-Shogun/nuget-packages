---
params:
    - name: configuration
      description: Configuration carrying the `Email` section. An absent section binds successfully and fails validation instead, which is what reports the missing token rather than a binding error.
      type: IConfiguration

returns: The `IServiceCollection` instance with the Resend client, template loader, and mail service registered.
---

# AddResendEmail

Registers the Resend email services and binds the `Email` configuration section. The method binds [`EmailSettings`](../configuration), configures the Resend API token, registers the Resend client behind a typed `HttpClient` with the standard resilience handler, and exposes the package mail sender through [`IResendMailService`](../services/resend-mail-service).

Call it once during startup, then depend on [`IResendMailService`](../services/resend-mail-service) and send classes that inherit from [`BaseMailTemplate`](../types/base-mail-template).

The mail service is registered as transient and the template loader as a singleton, so the file cache is shared across sends. Settings are bound through `IOptions<EmailSettings>`, so their values are fixed for the life of the process and a configuration reload requires a restart.

## Usage

::: warning
Requires an `Email` section in application configuration, usually from `appsettings.json`. `ApiToken` and `FromEmail` have no default, so an absent section fails validation while the host starts.
:::

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
