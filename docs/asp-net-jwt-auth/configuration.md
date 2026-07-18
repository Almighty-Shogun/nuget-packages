# Configuration

ASP.NET JWT Auth reads the `Auth` section from `appsettings.json`. The section is bound to `AuthSettings` during `AddApiAuth`, and `AddJwtBearerAuthentication` reads the same section when it configures JWT bearer validation.

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

See [AuthSettings](./configuration/auth-settings) for field descriptions and defaults.
