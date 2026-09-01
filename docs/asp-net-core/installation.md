# Installation

Install `AlmightyShogun.AspNet.Core` in the ASP.NET Core application, or in a package that builds on it. The package targets `net10.0` and references the ASP.NET Core shared framework. Every other package in this repository that returns an error response depends on it, so it is usually already present transitively.

```sh
dotnet add package AlmightyShogun.AspNet.Core
```

## Dependencies

### Framework references

- `Microsoft.AspNetCore.App` &mdash; the ASP.NET Core shared framework.

### Package references

- `UAParser` `3.1.47` &mdash; parses User-Agent headers for [`UserAgent`](./records/user-agent).

### Project references

- `AlmightyShogun.AspNet.Localization` &mdash; resolves the localized description on every error body, so it arrives transitively and its `Localization` section applies here too.

## Startup Registration

Each concern is registered on its own, so an application takes only what it needs. [`AddHttpErrorResponseWriter`](./extensions/add-http-error-response-writer) is the one everything else here depends on, and [`UseHttpErrorResponses`](./extensions/use-http-error-responses) adds the middleware that runs the handler chain and fills in empty error responses.

::: warning
Error bodies carry a localized description, so [`AddMessageLocalization`](/asp-net-localization/extensions/add-message-localization) is required alongside these calls. It lives in `AlmightyShogun.AspNet.Localization` and is not registered here.
:::

```csharp
using AlmightyShogun.AspNet.Core;
using AlmightyShogun.AspNet.Localization;

builder.Services
    .AddMessageLocalization(builder.Configuration)
    .AddHttpErrorResponseWriter()
    .AddExceptionHandling();

WebApplication app = builder.Build();

app.UseHttpErrorResponses();
```
