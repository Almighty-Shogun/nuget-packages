---
fields:
    - name: AllowedOrigins
      description: The optional `AllowedOrigins` section, read directly by [`AddCorsPolicy`](./extensions/add-cors-policy) rather than bound to a settings record. An absent section leaves the policy with no permitted origin, which blocks every cross-origin caller.
      fields:
          - name: AllowedOrigins
            description: Origins permitted by the CORS policy. The `*` wildcard is rejected, because browsers refuse it when credentials are allowed.
            type: 'string[]'
            default: '[]'
---

# Configuration

The `AllowedOrigins` section lists the origins the CORS policy permits. It is a bare array read directly rather than bound to a settings record, which is why it has no type of its own, and it is only needed by an application that serves cross-origin callers.

```json
{
    "AllowedOrigins": [
        "https://app.example.com",
        "https://admin.example.com"
    ]
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
