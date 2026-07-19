---
fields:
    - name: AllowedOrigins
      description: Array of allowed browser origins used by `AddAllowedOrigins`. Each value should include the scheme and host, and include the port when the application uses a non-default port.
      type: string[]
      default: '[]'

    - name: DefaultLanguage
      description: Default language code used by HTTP message resolution when the request does not provide `Accept-Language` or no requested-language message exists.
      type: string
      default: en
---

# Configuration

ASP.NET Utils reads configuration directly from `appsettings.json` for startup helpers that need application-level values. `AddAllowedOrigins` reads `AllowedOrigins`, and `AddHttpErrorResponses` reads `DefaultLanguage`.

```json [appsettings.json]
{
    "AllowedOrigins": [
        "https://app.example.com",
        "https://admin.example.com"
    ],
    "DefaultLanguage": "en"
}
```

<FrontmatterDocs/>
