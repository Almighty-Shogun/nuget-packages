# Installation

Install `AlmightyShogun.AspNet.Utils` in the ASP.NET Core API that needs request helpers, CORS setup, MVC action filters, cookie cleanup, language header helpers, User-Agent parsing, or standardized HTTP error responses. The package targets `net10.0`, uses ASP.NET Core framework APIs, and depends on `UAParser` for parsing raw User-Agent header values.

```sh
dotnet add package AlmightyShogun.AspNet.Utils
```

## Dependencies

### Framework references

- `Microsoft.AspNetCore.App` &mdash; provides HTTP context, response cookies, MVC filters, controller registration, CORS, and request header APIs used by the package.

### Package references

- `UAParser` `3.1.47` &mdash; parses User-Agent strings into browser, operating system, and device information.

## Startup Registration

Register only the helpers the application needs. [`AddActionFilters`](./extensions/add-action-filters) adds MVC controllers and captures [`SessionContext`](./records/session-context) before controller actions run. [`AddAllowedOrigins`](./extensions/add-allowed-origins) registers a named CORS policy from configuration. [`AddHttpErrorResponses`](./extensions/add-http-error-responses) reads `DefaultLanguage` from application configuration and registers the message resolver, MVC error-response filter, and exception handler used by [`UseHttpErrorResponses`](./extensions/use-http-error-responses).

When `AllowedOrigins` is missing or empty, the CORS policy is still registered but no browser origins are added to it. Configure explicit origins before enabling that policy in production, especially because the helper enables credentials.

::: code-group

```csharp [Program.cs]
using AlmightyShogun.AspNet.Utils;

builder.Services
    .AddActionFilters()
    .AddAllowedOrigins("DefaultCors", builder.Configuration)
    .AddHttpErrorResponses(builder.Configuration);

WebApplication app = builder.Build();

app.UseHttpErrorResponses();
app.MapControllers();

await app.RunAsync();
```

```json [appsettings.json]
{
    "AllowedOrigins": [
        "https://app.example.com"
    ],
    "DefaultLanguage": "en"
}
```

:::
