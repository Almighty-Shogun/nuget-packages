---
params:
    - name: configuration
      description: Application configuration, read for the optional `HttpErrors` section.
      type: IConfiguration

returns: The same `IServiceCollection` instance with the response writer registered.
---

# AddHttpErrorResponseWriter

Registers [`IHttpErrorResponseWriter`](../services/http-error-response-writer), the one place in the package set that formats an error body, together with the [`HttpErrorSettings`](../configuration/http-error-settings) it reads.

Every package that returns an error response depends on this, including the exception handlers, the MVC error filter, and the validation and maintenance packages. The `HttpErrors` section is optional and every value has a default, so an absent section leaves the package shape and logging behavior in place.

## Usage

```csharp
using AlmightyShogun.AspNet.Utils;

builder.Services.AddHttpErrorResponseWriter(builder.Configuration);
```

::: tip
To replace the writer with your own implementation, register it after this call, because the last registration for a service type wins.
:::

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection AddHttpErrorResponseWriter(
    IConfiguration configuration
);
```
