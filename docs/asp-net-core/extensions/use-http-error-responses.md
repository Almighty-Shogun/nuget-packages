---
returns: The same `IApplicationBuilder` instance with HTTP error responses configured.
---

# UseHttpErrorResponses

Completes the standardized error handling, running the exception handlers registered through [`AddExceptionHandling`](./add-exception-handling) and its siblings.

Two things are added, in order: the exception handler middleware that runs the registered handler chain, and a status code pages handler that fills in a body for any error response that has a status code but no content.

The second is what turns a bare `return NotFound();` into a full error body without the endpoint doing anything, whether MVC produced the response or something below MVC did.

## Usage

::: code-group

```csharp [Program.cs]
using AlmightyShogun.AspNet.Core;

WebApplication app = builder.Build();

app.UseHttpErrorResponses();
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
    "errorDescription": "The requested resource was not found."
}
```

::: tip
`errorDescription` is resolved from the [`http-error.{status}`](../http-error-messages) message key in the language the caller negotiated. A status with no entry in that language falls back to the key itself, so the client sees `http-error.404`.
:::

## Pipeline order

Call it early, before routing and before endpoints, so it wraps the rest of the pipeline. An exception thrown by middleware registered *before* this call is not handled by it.

```csharp
using AlmightyShogun.AspNet.Core;

WebApplication app = builder.Build();

app.UseForwardedHeaders();
app.UseMessageLocalization();
app.UseHttpErrorResponses();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
```

<FrontmatterDocs/>

## Type signature

```csharp
public IApplicationBuilder UseHttpErrorResponses();
```
