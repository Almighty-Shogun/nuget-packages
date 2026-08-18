# Installation

Install `AlmightyShogun.AspNet.Utils` in the ASP.NET Core application, or in a package that builds on it. The package targets `net10.0` and references the ASP.NET Core shared framework. Every other package in this repository that returns an error response depends on it, so it is usually already present transitively.

```sh
dotnet add package AlmightyShogun.AspNet.Utils
```

## Dependencies

### Framework references

- `Microsoft.AspNetCore.App` &mdash; the ASP.NET Core shared framework.

### Package references

- `UAParser` `3.1.47` &mdash; parses User-Agent headers for [`UserAgent`](./records/user-agent).

### Project references

- `AlmightyShogun.Utils` &mdash; supplies the configuration binding helper both settings sections are bound through, so a missing or malformed value fails at startup.

## Startup Registration

Each concern is registered on its own, so an application takes only what it needs. [`AddMessageLocalization`](./extensions/add-message-localization) and [`AddHttpErrorResponseWriter`](./extensions/add-http-error-response-writer) are the two everything else depends on. [`UseHttpErrorResponses`](./extensions/use-http-error-responses) adds the middleware that runs the handler chain and fills in empty error responses; [`UseMessageLocalization`](./extensions/use-message-localization) sets `Content-Language`.

::: warning
Call [`UseMessageLocalization`](./extensions/use-message-localization) before [`UseHttpErrorResponses`](./extensions/use-http-error-responses), so the header is set on error responses too.
:::

```csharp
using AlmightyShogun.AspNet.Utils;

builder.Services
    .AddMessageLocalization(builder.Configuration)
    .AddHttpErrorResponseWriter(builder.Configuration)
    .AddExceptionHandling()
    .AddHttpErrorResponseFilter();

WebApplication app = builder.Build();

app.UseMessageLocalization();
app.UseHttpErrorResponses();
```
