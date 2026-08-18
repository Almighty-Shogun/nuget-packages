---
fields:
    - name: IpAddress
      description: The client IP address from the connection, or `null` when it is not available.
      type: string?
      default: 'null'

    - name: UserAgent
      description: The raw User-Agent header value.
      type: string?
      default: 'null'
---

# SessionContext

Request metadata captured for the current HTTP request, read through [`GetSessionContext`](../extensions/get-session-context). Both values are a snapshot rather than a live read, so the record can be handed to a background job or an audit record after the request has ended. `UserAgent` is the header exactly as sent, unparsed; call [`GetUserAgent`](../extensions/get-user-agent) when the browser or device is what matters.

::: warning
`IpAddress` comes from `HttpContext.Connection.RemoteIpAddress` and never from a request header. A header such as `X-Forwarded-For` is trivially forged by the client, and this value is persisted for audit by other packages.

For an application behind a proxy, configure forwarded headers with [`AddCloudflareHeaders`](../extensions/add-cloudflare-headers) and `app.UseForwardedHeaders()`. That rewrites the connection address from a trusted proxy only, so this value stays accurate without becoming forgeable.
:::

## Usage

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.Utils;

[ApiController]
[Route("sessions")]
public sealed class SessionsController : ControllerBase
{
    [HttpPost]
    public IActionResult Create()
    {
        SessionContext sessionContext = HttpContext.GetSessionContext();

        return Ok(new 
        {
            sessionContext.IpAddress,
            sessionContext.UserAgent
        });
    }
}
```

## ItemKey

The `HttpContext.Items` key the context is stored under by the session context filter. Useful when writing middleware that needs to read or replace the stored value.

### Type signature

```csharp
public const string ItemKey = nameof(SessionContext);
```

<FrontmatterDocs/>
