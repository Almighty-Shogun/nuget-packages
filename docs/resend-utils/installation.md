# Installation

Install `AlmightyShogun.Resend.Utils` in the application that sends email through Resend. The package targets `net10.0`, uses the official `Resend` client package, and registers its mail sender through `Microsoft.Extensions.DependencyInjection`.

```sh
dotnet add package AlmightyShogun.Resend.Utils
```

## Dependencies

### Package references

- `Resend` `0.6.0` &mdash; provides the Resend API client used to send email messages.

### Project references

- `AlmightyShogun.Logging` &mdash; provides shared logging behavior used across the package set.
- `AlmightyShogun.Utils` &mdash; provides the configuration binding helper used during startup registration.

## Startup Registration

Register the package once during application startup. The registration binds email settings, configures the Resend API token, and exposes the mail sender through [`IResendMailService`](./services/resend-mail-service).

::: warning
Requires an `Email` section in application configuration, usually from `appsettings.json`.
:::

```csharp
using AlmightyShogun.Resend.Utils;

builder.Services.AddResendEmail(builder.Configuration);
```

The package reads `BaseEmailTemplate.html`, `BaseEmailParagraph.html`, and `BaseEmailButton.html` from a `mail` folder under `AppContext.BaseDirectory`. When the application is published, make sure those files are copied to that folder so emails can be rendered before they are sent.
