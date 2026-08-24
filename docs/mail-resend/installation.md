# Installation

Install `AlmightyShogun.Mail.Resend` in the application that sends email through Resend. The package targets `net10.0`, uses the official `Resend` client package, and registers its mail sender through `Microsoft.Extensions.DependencyInjection`.

```sh
dotnet add package AlmightyShogun.Mail.Resend
```

## Dependencies

### Package references

- `Resend` `0.8.0` &mdash; provides the Resend API client used to send email messages.
- `Microsoft.Extensions.Http` `10.0.11` &mdash; registers the Resend client through `IHttpClientFactory`.
- `Microsoft.Extensions.Http.Resilience` `10.9.0` &mdash; adds retry, a circuit breaker, and a timeout to the send call.
- `Microsoft.Extensions.DependencyInjection.Abstractions` `10.0.11` &mdash; the service collection the mail service registers into.
- `Microsoft.Extensions.Logging.Abstractions` `10.0.11` &mdash; `ILogger<T>`, used to report a failed send.
- `Microsoft.Extensions.Options` `10.0.11` &mdash; reads the bound `Email` settings at runtime.

### Project references

- `AlmightyShogun.Core` &mdash; provides the configuration binding helper used during startup registration.

## Startup Registration

Register the package once during application startup. The registration binds email settings, configures the Resend API token, and exposes the mail sender through [`IResendMailService`](./services/resend-mail-service).

::: warning
Requires an `Email` section in application configuration, usually from `appsettings.json`. `ApiToken` and `FromEmail` have no default, so an absent section fails validation while the host starts.
:::

```csharp
using AlmightyShogun.Mail.Resend;

builder.Services.AddResendEmail(builder.Configuration);
```
