---
fields:
    - name: AllowedOrigins
      description: The optional `AllowedOrigins` section, read directly by [`AddCorsPolicy`](./extensions/add-cors-policy) rather than bound to a settings record. An absent section leaves the policy with no permitted origin, which blocks every cross-origin caller.
      fields:
          - name: AllowedOrigins
            description: Origins permitted by the CORS policy. The `*` wildcard is rejected, because browsers refuse it when credentials are allowed.
            type: 'string[]'
            default: '[]'
          - name: AllowedHeaders
            description: Request headers the CORS policy permits. An empty list allows any header.
            type: 'string[]'
            default: '[]'
          - name: AllowedMethods
            description: HTTP methods the CORS policy permits. An empty list allows any method.
            type: 'string[]'
            default: '[]'
---

# Configuration

The `AllowedOrigins` section lists the origins the CORS policy permits, and the optional `AllowedHeaders` and `AllowedMethods` sections narrow what those origins may send. All three are bare arrays read directly rather than bound to a settings record, which is why they have no type of their own, and they are only needed by an application that serves cross-origin callers.

Leaving `AllowedHeaders` or `AllowedMethods` out allows any header or any method.

```json
{
    "AllowedOrigins": [
        "https://app.example.com",
        "https://admin.example.com"
    ],
    "AllowedMethods": ["GET", "POST"],
    "AllowedHeaders": ["Content-Type", "Authorization"]
}
```

::: warning
Listing `*` does not work. Browsers refuse the wildcard on a policy that allows credentials, so the value is rejected rather than silently widening the policy.
:::

<FrontmatterDocs/>

## Usage

The section binds to no record, so an application that needs the list itself reads it straight from configuration.

```csharp
using Microsoft.Extensions.Configuration;

public sealed class OriginReporter(IConfiguration configuration)
{
    public string[] GetAllowedOrigins()
        => configuration.GetSection("AllowedOrigins").Get<string[]>() ?? [];
}
```
