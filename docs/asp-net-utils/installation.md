# Installation

Install `AlmightyShogun.AspNet.Utils` in the ASP.NET Core API that needs request helpers, CORS setup, MVC action filters, cookie cleanup, or User-Agent parsing. The package targets `net10.0`, uses ASP.NET Core framework APIs, and depends on `UAParser` for parsing raw User-Agent header values.

```sh
dotnet add package AlmightyShogun.AspNet.Utils
```

## Dependencies

- `Microsoft.AspNetCore.App` framework reference &mdash; provides HTTP context, response cookies, MVC filters, controller registration, CORS, and request header APIs used by the package.
- `UAParser` `3.1.47` &mdash; parses User-Agent strings into browser, operating system, and device information.

## Startup Registration

Register action filters when the application wants `SessionContextFilter` to populate request context before controller actions run. Register allowed origins when the application wants CORS origins to come from the `AllowedOrigins` configuration section.

::: warning
Requires an `AllowedOrigins` section in application configuration, usually from `appsettings.json`.
:::

```csharp
using AlmightyShogun.AspNet.Utils;

builder.Services
    .AddActionFilters()
    .AddAllowedOrigins("DefaultCors", builder.Configuration);
```
