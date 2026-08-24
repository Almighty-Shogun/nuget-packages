---
returns: The client address, or `null` when the connection has none.
---

# GetIpAddress

Gets the client address for the current request from the connection, normalized so an IPv4 address tunneled as IPv4-mapped IPv6 comes back in its IPv4 form. Without that normalization the same client can appear as `::ffff:203.0.113.10` or `203.0.113.10` depending on how the socket was opened, which breaks equality checks and allow lists.

Returns `null` when the connection has no address, which happens on an in-memory test server.

## Usage

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.Core;

[ApiController]
[Route("audit")]
public sealed class AuditController(IAuditLog auditLog) : ControllerBase
{
    [HttpPost]
    public IActionResult Record()
    {
        auditLog.Write(HttpContext.GetIpAddress() ?? "unknown");

        return Ok();
    }
}
```

::: warning
The address comes from the connection, never from a forwarded header, because a header-supplied address is chosen by the caller and cannot be trusted. Behind a proxy or CDN the connection address is the proxy until forwarded headers have been processed, so call `UseForwardedHeaders` first and see [`AddCloudflareHeaders`](./add-cloudflare-headers).
:::

<FrontmatterDocs/>

## Type signature

```csharp
public string? GetIpAddress();
```
