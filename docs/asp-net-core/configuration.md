---
fields:
    - name: CORS policy
      description: Three root-level sections read directly by [`AddCorsPolicy`](./extensions/add-cors-policy) rather than bound to a settings record, which is why they have no type of their own. The policy they build always allows credentials.
      fields:
          - name: AllowedOrigins
            description: Origins permitted by the CORS policy. Because the policy allows credentials, a `*` entry is refused at startup rather than silently widening the policy. Absent or empty permits no cross-origin caller at all.
            type: 'string[]'
            default: '[]'
          - name: AllowedHeaders
            description: Request headers the CORS policy permits. Absent or empty permits every header.
            type: 'string[]'
            default: '[]'
          - name: AllowedMethods
            description: HTTP methods the CORS policy permits. Absent or empty permits every method.
            type: 'string[]'
            default: '[]'
---

# Configuration

The `AllowedOrigins` section lists the origins the CORS policy permits, and the optional `AllowedHeaders` and `AllowedMethods` sections narrow what those origins may send. All three are bare arrays read directly rather than bound to a settings record, and they are only needed by an application that serves cross-origin callers.

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

::: danger
The three sections fail in opposite directions. Omit `AllowedOrigins` and no cross-origin caller is permitted at all; omit `AllowedHeaders` or `AllowedMethods` and every header or every method is permitted. Narrowing what origins may send means listing the values, never leaving the section out.
:::

<FrontmatterDocs/>
