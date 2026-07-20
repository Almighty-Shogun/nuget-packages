---
returns: The `IServiceCollection` instance with ASP.NET validation services configured.
---

# AddAspNetValidation

Registers the services used by ASP.NET Validation. The method adds validation exception handling, the default validation response factory, rule caching, request validators, MVC resource/action filters, endpoint filters, and controller behavior that turns invalid model state into a standardized validation response.

Call this method after [`AddHttpErrorResponses`](/asp-net-utils/extensions/add-http-error-responses), because validation responses resolve message keys through ASP.NET Utils. The registration does not bind package-specific `appsettings.json` configuration; localization comes from message files loaded by ASP.NET Utils.

## Usage

```csharp
using AlmightyShogun.AspNet.Utils;
using AlmightyShogun.AspNet.Validation;

builder.Services
    .AddHttpErrorResponses(builder.Configuration)
    .AddAspNetValidation();
```

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection AddAspNetValidation();
```
