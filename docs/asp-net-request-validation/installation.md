# Installation

Install `AlmightyShogun.AspNet.RequestValidation` in the ASP.NET Core application that validates incoming requests. The package targets `net10.0`.

```sh
dotnet add package AlmightyShogun.AspNet.RequestValidation
```

## Dependencies

### Framework references

- `Microsoft.AspNetCore.App` &mdash; the ASP.NET Core shared framework.

### Project references

- `AlmightyShogun.AspNet.Core` &mdash; provides the shared error response shape.
- `AlmightyShogun.AspNet.Localization` &mdash; provides message resolution.

## Startup Registration

[`AddAspNetValidation`](./extensions/add-asp-net-validation) registers the rule cache, filters, and exception handler. [`UseAspNetValidation`](./extensions/use-asp-net-validation) adds the middleware, and has overloads for minimal API endpoints and route groups.

::: warning
[`AddMessageLocalization`](/asp-net-localization/extensions/add-message-localization) and [`AddHttpErrorResponseWriter`](/asp-net-core/extensions/add-http-error-response-writer) must also be called. Without them, validation messages resolve to their raw keys such as `validation.required`.
:::

```csharp
using AlmightyShogun.AspNet.Core;
using AlmightyShogun.AspNet.Localization;
using AlmightyShogun.AspNet.RequestValidation;

builder.Services
    .AddMessageLocalization(builder.Configuration)
    .AddHttpErrorResponseWriter()
    .AddExceptionHandling()
    .AddHttpErrorResponseFilter()
    .AddAspNetValidation();

WebApplication app = builder.Build();

app.UseHttpErrorResponses();
app.UseAspNetValidation();
```
