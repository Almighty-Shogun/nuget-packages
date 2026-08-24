# Exceptions

The package defines no exceptions of its own, and no handler for yours. It defines `IExceptionMapper`, which decides which exceptions you answer and what each one becomes on the wire, and [`ErrorMapping`](./records/error-mapping), the answer itself. Writing the handler that pairs them is yours to do, as it is for every package in this repository.

## IExceptionMapper

Turns one exception into the status code, error code, and message it should produce. The exception stays plain and carries only its own data, so a domain type never names an HTTP status or a message file key.

::: code-group

```csharp [AccountLockedException.cs]
public sealed class AccountLockedException(
    DateTimeOffset lockoutEnd
) : Exception
{
    public DateTimeOffset LockoutEnd { get; } = lockoutEnd;
}
```

```csharp [AppExceptionMapper.cs]
using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Core;

public sealed class AppExceptionMapper : IExceptionMapper
{
    public ErrorMapping? Map(Exception exception) => exception switch
    {
        AccountLockedException lockedOut => new ErrorMapping(
            StatusCodes.Status423Locked,
            "account_locked_out",
            "auth.locked-out",
            [lockedOut.LockoutEnd]
        ),

        _ => null
    };
}
```

```csharp [AppExceptionHandler.cs]
using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Core;
using Microsoft.AspNetCore.Diagnostics;

public sealed class AppExceptionHandler(
    AppExceptionMapper exceptionMapper,
    IMessageResolver messageResolver,
    IHttpErrorResponseWriter responseWriter
) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (httpContext.Response.HasStarted
            || exceptionMapper.Map(exception) is not { } mapping)
        {
            return false;
        }

        await responseWriter.WriteAsync(
            httpContext,
            mapping.StatusCode,
            mapping.Code,
            messageResolver.Resolve(
                mapping.MessageKey,
                mapping.MessageParameters
            ),
            cancellationToken
        );

        return true;
    }
}
```

:::

::: warning
Register your handler before [`AddExceptionHandling`](./extensions/add-exception-handling). Handlers run in registration order and the fallback there answers every exception, so a handler registered after it never runs.
:::

### Map

Called on the exception path of a failing request with whatever was thrown. Return an [`ErrorMapping`](./records/error-mapping) to answer, or `null` to decline, which is what lets the handler return `false` and pass the exception to the ones behind it.

A mapper is normally registered as a singleton and runs on a failing request, so it has to be thread-safe. Keep it a pattern match; it is the wrong place to read configuration or touch a database.

### Type signature

```csharp
public interface IExceptionMapper
{
    ErrorMapping? Map(Exception exception);
}
```
