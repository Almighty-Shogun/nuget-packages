---
returns: The `IApplicationBuilder` instance with standardized HTTP error response middleware configured.
---

# UseHttpErrorResponses

Adds the middleware and exception handling needed to write standardized HTTP error bodies. The method registers an exception handler for unhandled exceptions and runs middleware that fills empty error responses with an `HttpErrorResponse` body.

Use this method in the request pipeline after `AddHttpErrorResponses` has registered the required services. Place it before endpoint execution so exceptions and empty error status codes produced by downstream middleware, controllers, or endpoints can be normalized.

## Usage

```csharp
using AlmightyShogun.AspNet.Utils;

builder.Services.AddHttpErrorResponses(builder.Configuration);

WebApplication app = builder.Build();

app.UseHttpErrorResponses();
app.MapControllers();

await app.RunAsync();
```

<FrontmatterDocs/>

## Type signature

```csharp
public IApplicationBuilder UseHttpErrorResponses();
```
