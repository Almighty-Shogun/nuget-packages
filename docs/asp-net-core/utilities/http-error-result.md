# HttpErrorResult

Creates an MVC `ObjectResult` for a standardized [`HttpErrorResponse`](../records/http-error-response), with the result's status code taken from the response's `Code`.

Use it in a controller or filter that needs to return the standard error body as an action result. In middleware or a minimal API endpoint, use [`IHttpErrorResponseWriter`](../services/http-error-response-writer) instead, which writes directly to the response.

## Usage

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.Core;

[ApiController]
[Route("orders")]
public sealed class OrdersController(
    IMessageResolver messageResolver
) : ControllerBase
{
    [HttpGet("{id:int}")]
    public IActionResult Get(int id)
        => HttpErrorResult.Create(new HttpErrorResponse
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

## Create

Wraps the response in an `ObjectResult` whose `StatusCode` is the response's `Code`, so the two cannot disagree.

### Type signature

```csharp
public static ObjectResult Create(
    HttpErrorResponse response
);
```
