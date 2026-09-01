---
params:
    - name: clientContext
      description: The context to pin for the rest of the request, replacing anything stored before it.
      type: ClientContext
---

# SetClientContext

Stores a [`ClientContext`](../records/client-context) on the current request, so every later [`GetClientContext`](./get-client-context) returns it instead of reading the connection.

Use it from middleware that captures the values once per request, or from a test that needs fixed values with no real connection behind them. The `HttpContext.Items` key is private to the package, so this is the only supported way to seed the entry.

## Usage

```csharp
using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Core;

public sealed class ClientContextMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        context.SetClientContext(new ClientContext(
            context.GetIpAddress(),
            context.Request.Headers.UserAgent.ToString()
        ));

        await next(context);
    }
}
```

<FrontmatterDocs/>

## Type signature

```csharp
public void SetClientContext(ClientContext clientContext);
```
