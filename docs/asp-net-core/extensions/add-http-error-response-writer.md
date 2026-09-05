---
returns: The same `IServiceCollection` instance with the response writer registered.
---

# AddHttpErrorResponseWriter

Registers [`IHttpErrorResponseWriter`](../services/http-error-response-writer), which formats the standardized error body.

The exception handlers write through it, as do the validation and maintenance packages; [`HttpErrorResult`](../types/http-error-result) is the one exception, carrying the same shape through MVC's formatters without ever reaching the writer. It reads no configuration, because the body shape is fixed.

## Usage

```csharp
using AlmightyShogun.AspNet.Core;

builder.Services.AddHttpErrorResponseWriter();
```

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection AddHttpErrorResponseWriter();
```
