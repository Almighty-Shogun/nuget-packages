# Configuration

ASP.NET JWT Auth reads the `Auth` section from `appsettings.json`. The section is bound to [`AuthSettings`](./configuration/auth-settings) during [`AddJwtAuth`](./extensions/add-jwt-auth), and the package reads the same section when it configures JWT bearer validation.

The `Auth` section is required. [`AuthSettings`](./configuration/auth-settings) uses validation attributes, so `Issuer`, `Secret`, `Hours`, and `RefreshTokenDays` must be present and usable before the application starts successfully. When `Hosts` contains mappings, JWT audience validation is enabled for the configured app audiences and protected endpoints also require the token audience to match the current request host. When `Hosts` is empty, app-audience validation is disabled completely.

```json
{
    "Auth": {
        "Issuer": "https://auth.example.com",
        "Secret": "replace-with-a-long-random-secret",
        "Hours": 1,
        "RefreshTokenDays": 30,
        "LocalhostApp": "localhost",
        "Hosts": {
            "api.example.com": "api",
            "admin.example.com": "admin"
        }
    }
}
```
