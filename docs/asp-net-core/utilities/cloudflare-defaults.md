# CloudflareDefaults

Holds the published Cloudflare edge network ranges and the default client IP header, used by [`AddCloudflareHeaders`](../extensions/add-cloudflare-headers). Read `Networks` when an application needs the same list for its own checks, for example a firewall rule or a health endpoint that should only answer the edge. Both members are data only: nothing here configures the pipeline on its own.

## Usage

```csharp
using System.Net;
using AlmightyShogun.AspNet.Core;

bool fromCloudflare = CloudflareDefaults.Networks
    .Any(network => network.Contains(remoteAddress));
```

::: warning
Cloudflare changes the published ranges occasionally. A stale entry fails silently: the request is not recognized as coming from a trusted proxy, so the recorded client address is the Cloudflare edge rather than the real client.
:::

## ClientIpHeader

The header Cloudflare sets with the originating client address. Pass a different header to [`AddCloudflareHeaders`](../extensions/add-cloudflare-headers) only when something between the edge and the application rewrites it.

### Type signature

```csharp
public const string ClientIpHeader = "CF-Connecting-IP";
```

## Networks

The published Cloudflare IPv4 and IPv6 ranges, in the order Cloudflare lists them. An address outside every range reached the application without passing through the edge.

### Type signature

```csharp
public static IReadOnlyList<IPNetwork> Networks { get; }
```
