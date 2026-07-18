---
fields:
    - name: AllowedOrigins
      description: Array of allowed browser origins. Each value should include the scheme and host, and include the port when the application uses a non-default port.
      type: string[]
---

# Configuration

ASP.NET Utils reads the `AllowedOrigins` section from `appsettings.json` when `AddAllowedOrigins` registers a CORS policy. The section is an array of exact origins that should be allowed to call the API.

```json
{
    "AllowedOrigins": [
        "https://app.example.com",
        "https://admin.example.com"
    ]
}
```

<FrontmatterDocs/>
