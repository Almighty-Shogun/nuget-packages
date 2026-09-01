# HttpErrorResult

An MVC `ObjectResult` carrying a standardized [`HttpErrorResponse`](../records/http-error-response), with the result's status code taken from the response's `Code`.

Use it in a controller or filter that needs to return the standard error body as an action result. In middleware or a minimal API endpoint, use [`IHttpErrorResponseWriter`](../services/http-error-response-writer) instead, which writes directly to the response.

## Usage

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.Core;
using AlmightyShogun.AspNet.Localization;

[ApiController]
[Route("orders")]
public sealed class OrdersController(
    IMessageResolver messageResolver
) : ControllerBase
{
    [HttpGet("{id:int}")]
    public IActionResult Get(int id)
        => new HttpErrorResult(new HttpErrorResponse
        {
            Code = StatusCodes.Status404NotFound,
            Error = "order_not_found",
            ErrorDescription = messageResolver.Resolve(
                "orders.not-found",
                [id]
            )
        });
}
```

::: tip
This result is serialized by MVC's formatters, so its property casing comes from `AddJsonOptions`, while [`IHttpErrorResponseWriter`](../services/http-error-response-writer) uses `ConfigureHttpJsonOptions`. Move both together, or the same error reaches clients spelled two ways.
:::

## Constructor

Wraps the response in a result whose `StatusCode` is the response's `Code`, so the two cannot disagree.

### Type signature

```csharp
public HttpErrorResult(
    HttpErrorResponse response
);
```
