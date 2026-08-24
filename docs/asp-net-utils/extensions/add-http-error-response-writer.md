---
returns: The same `IServiceCollection` instance with the response writer registered.
---

# AddHttpErrorResponseWriter

Registers [`IHttpErrorResponseWriter`](../services/http-error-response-writer), the one place in the package set that formats an error body.

Every package that returns an error response depends on this, including the exception handlers, the MVC error filter, and the validation and maintenance packages. It reads no configuration, because the body shape is fixed.

## Usage

```csharp
using AlmightyShogun.AspNet.Utils;

builder.Services.AddHttpErrorResponseWriter();
```

::: tip
To replace the writer with your own implementation, register it after this call, because the last registration for a service type wins.
:::

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection AddHttpErrorResponseWriter();
```
