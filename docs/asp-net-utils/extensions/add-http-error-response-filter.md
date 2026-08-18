---
returns: The same `IServiceCollection` instance with the error response filter registered.
---

# AddHttpErrorResponseFilter

Registers the MVC filter that fills in a standardized body for an error result carrying a status code but no content, so a bare `return NotFound();` returns a full error response without the action doing anything. A result that already carries a value is left alone, so an action returning its own error body keeps it. It covers only results MVC produces; an error raised below MVC needs [`UseHttpErrorResponses`](./use-http-error-responses).

## Usage

::: code-group

```csharp [Program.cs]
using AlmightyShogun.AspNet.Utils;

builder.Services.AddHttpErrorResponseFilter();
```

```csharp [OrdersController.cs]
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("orders")]
public sealed class OrdersController : ControllerBase
{
    [HttpGet("{id:int}")]
    public IActionResult Get(int id) => NotFound();
}
```

:::

The empty `NotFound()` above returns:

```json
{
    "code": 404,
    "error": "not_found",
    "errorDescription": "http-error.404"
}
```

::: warning
Requires [`AddMessageLocalization`](./add-message-localization) and [`AddHttpErrorResponseWriter`](./add-http-error-response-writer). It applies to MVC results only; a minimal API endpoint returning `Results.NotFound()` is unaffected.
:::

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection AddHttpErrorResponseFilter();
```
