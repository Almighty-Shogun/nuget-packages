---
params:
    - name: configuration
      description: Application configuration containing the `Email` section.
      type: IConfiguration

returns: The `IServiceCollection` instance with Resend email services configured.
---

# AddResendEmail

Registers Resend email sending services and configuration. The method binds [`EmailSettings`](../configuration/email-settings), configures the Resend API token, registers the Resend client, and exposes the package mail sender through [`IResendMailService`](../services/resend-mail-service).

Call this method once during application startup before resolving services that send email. After registration, application services should depend on [`IResendMailService`](../services/resend-mail-service) and send classes that inherit from [`BaseMailTemplate`](../types/base-mail-template).

## Usage

::: warning
Requires an `Email` section in application configuration, usually from `appsettings.json`.
:::

```csharp
using AlmightyShogun.Resend.Utils;

builder.Services.AddResendEmail(builder.Configuration);
```

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection AddResendEmail(
    IConfiguration configuration
);
```
