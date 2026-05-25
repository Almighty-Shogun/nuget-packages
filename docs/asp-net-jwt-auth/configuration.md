---
outline: deep

fields:
    - name: Issuer
      description: The exact issuer value expected in incoming JWT access tokens.
      type: string

    - name: Secret
      description: The symmetric signing secret used to validate incoming JWTs. Use a long secret and keep it outside source control for real environments.
      type: string

    - name: Hours
      description: The access-token lifetime in hours. The package exposes this value through `AuthSettings`; token creation code can use it when issuing access tokens.
      type: int

    - name: RefreshTokenDays
      description: The refresh-token cookie lifetime in days. Pass this value to `SetRefreshTokenCookie` when writing the refresh-token cookie.
      type: int

    - name: LocalhostApp
      description: The application audience to use when a request arrives on `localhost`.
      type: string?
      default: 'null'

    - name: Hosts
      description: Map of request host names to application audience names. During token validation, the package resolves the current host to an app name and requires the JWT audience to contain that app.
      type: 'Dictionary<string, string>'
      default: '[]'
---

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

<FrontmatterDocs/>
