# Installation

Install `AlmightyShogun.AspNet.Validation` in the ASP.NET Core API that should validate request DTOs and return standardized validation error responses. The package targets `net10.0`, uses ASP.NET Core MVC and minimal API infrastructure, and expects ASP.NET Utils HTTP error response services to be registered before validation services.

```sh
dotnet add package AlmightyShogun.AspNet.Validation
```

## Dependencies

### Framework references

- `Microsoft.AspNetCore.App` &mdash; provides MVC filters, endpoint filters, routing, exception handling, HTTP abstractions, and object result types used by the package.

### Project references

- `AlmightyShogun.AspNet.Utils` &mdash; provides [`IMessageResolver`](/asp-net-utils/services/message-resolver), [`HttpErrorResponse`](/asp-net-utils/records/http-error-response), and [`HttpErrorResult`](/asp-net-utils/types/http-error-result).

## Startup Registration

Register ASP.NET Utils error responses first, then register validation. [`AddAspNetValidation`](./extensions/add-asp-net-validation) adds the validation filters, response factories, exception handling integration, and controller behavior. [`UseAspNetValidation`](./extensions/use-asp-net-validation) adds middleware that converts validation exceptions and invalid request bodies into standardized responses.

::: code-group

```csharp [Controllers.cs]
using AlmightyShogun.AspNet.Utils;
using AlmightyShogun.AspNet.Validation;

builder.Services
    .AddHttpErrorResponses(builder.Configuration)
    .AddAspNetValidation();

WebApplication app = builder.Build();

app.UseHttpErrorResponses();
app.UseAspNetValidation();

app.MapControllers();
```

```csharp [MinimalApis.cs]
using AlmightyShogun.AspNet.Utils;
using AlmightyShogun.AspNet.Validation;

builder.Services
    .AddHttpErrorResponses(builder.Configuration)
    .AddAspNetValidation();

WebApplication app = builder.Build();

app.UseHttpErrorResponses();
app.UseAspNetValidation();

app.MapPost("/projects", (CreateProjectRequest request) => Results.Ok(request))
    .UseAspNetValidation();
```

:::
