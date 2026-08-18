---
params:
    - name: clientIpHeader
      description: Header carrying the originating client address.
      type: string
      default: CF-Connecting-IP
    - name: additionalNetworks
      description: Extra trusted networks, for an application behind Cloudflare and an internal load balancer.
      type: IEnumerable<IPNetwork>?
      default: 'null'
    - name: forwardLimit
      description: Number of proxy hops to walk. `null` walks every trusted hop.
      type: int?
      default: 'null'

returns: The same `IServiceCollection` instance with forwarded headers configured.
---

# AddCloudflareHeaders

Trusts the Cloudflare edge network for forwarded headers, so `HttpContext.Connection.RemoteIpAddress` becomes the real client address and `HttpRequest.IsHttps` reflects the original scheme rather than the connection between Cloudflare and the origin.

Without this, an application behind Cloudflare sees every request as coming from a Cloudflare IP over plain HTTP. That breaks IP-based audit logging, rate limiting, and any code that checks `IsHttps` before setting a secure cookie.

::: danger
This method **only configures options**. Nothing happens until the middleware runs: `app.UseForwardedHeaders();` Omit it and the helper silently does nothing &mdash; no error, no warning, just the proxy's address in every log.
:::

## Usage

::: code-group

```csharp [Program.cs]
using AlmightyShogun.AspNet.Utils;

builder.Services.AddCloudflareHeaders();

WebApplication app = builder.Build();

app.UseForwardedHeaders();
app.UseHttpErrorResponses();
```

```csharp [BehindLoadBalancer.cs]
using System.Net;
using AlmightyShogun.AspNet.Utils;

builder.Services.AddCloudflareHeaders(
    additionalNetworks: [IPNetwork.Parse("10.0.0.0/8")]
);
```

:::

## Trusted networks

Trust is restricted to [Cloudflare's published ranges](../utilities/cloudflare), so `X-Forwarded-For` is honored from the edge and ignored from anywhere else. ASP.NET Core's forwarded-headers middleware trusts nothing by default, because an application that trusts the header from any source lets a client claim any IP address it likes.

Existing `KnownIPNetworks` and `KnownProxies` entries are cleared, so the trusted set is exactly what this method configures plus `additionalNetworks`.

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection AddCloudflareHeaders(
    string clientIpHeader = Cloudflare.ClientIpHeader,
    IEnumerable<IPNetwork>? additionalNetworks = null,
    int? forwardLimit = null
);
```
