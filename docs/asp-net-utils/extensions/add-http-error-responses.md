---
params:
    - name: configuration
      description: Application configuration containing the optional `DefaultLanguage` value.
      type: IConfiguration

returns: The `IServiceCollection` instance with standardized HTTP error response services configured.
---

# AddHttpErrorResponses

Registers the services used to write standardized JSON error responses. The method reads `DefaultLanguage` from application configuration, adds an HTTP context accessor, registers the internal language provider, registers the JSON-backed message resolver, registers the exception handler for `HttpErrorException`, and adds an MVC result filter that converts empty error results into an `HttpErrorResponse` body.

Use this method when an API should return consistent error objects for empty MVC error results, unhandled exceptions, and explicitly thrown `HttpErrorException` values. `DefaultLanguage` controls the fallback language used when the request does not provide an `Accept-Language` header or when no requested-language message exists.

## Usage

::: code-group

```csharp [Program.cs]
using AlmightyShogun.AspNet.Utils;

builder.Services
    .AddActionFilters()
    .AddHttpErrorResponses(builder.Configuration);

WebApplication app = builder.Build();

app.UseHttpErrorResponses();
app.MapControllers();

await app.RunAsync();
```

```json [appsettings.json]
{
    "DefaultLanguage": "en"
}
```

:::

If the application does not use `AddActionFilters`, make sure MVC controller services are still registered through `AddControllers` or the application's normal MVC setup. Add HTTP error message files as described on the [Localization](../localization) page.

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection AddHttpErrorResponses(
    IConfiguration configuration
);
```
