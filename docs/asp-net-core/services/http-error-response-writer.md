# HttpErrorResponseWriter

Writes the standardized error response body and sets the status code, so every failure reaches the client as one shape no matter which layer produced it. Application code depends on `IHttpErrorResponseWriter`. A response that has already started is left untouched rather than throwing, which is what makes it safe to call from an exception handler.

## WriteAsync

Sets the response status code and writes the [`HttpErrorResponse`](../records/http-error-response) body as JSON.

Does nothing when the response has already started. Check `HttpResponse.HasStarted` if the caller needs to know whether the write happened.

```csharp
using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Core;
using AlmightyShogun.AspNet.Localization;

public sealed class QuotaMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(
        HttpContext context,
        IQuotaPolicy quotaPolicy,
        IMessageResolver messageResolver,
        IHttpErrorResponseWriter errorResponseWriter
    )
    {
        if (await quotaPolicy.IsOverQuotaAsync(context))
        {
            await errorResponseWriter.WriteAsync(
                context,
                StatusCodes.Status429TooManyRequests,
                "quota_exceeded",
                messageResolver.Resolve("quota.exceeded"),
                context.RequestAborted
            );

            return;
        }

        await next(context);
    }
}
```

### Type signature

```csharp
public Task WriteAsync(
    HttpContext context,
    int statusCode,
    string errorCode,
    string? description,
    CancellationToken cancellationToken = default
);
```
