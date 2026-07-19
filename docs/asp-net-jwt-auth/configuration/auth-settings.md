---
fields:
    - name: Issuer
      description: Required expected JWT issuer value.
      type: string

    - name: Secret
      description: Required symmetric signing secret used to validate JWT access tokens.
      type: string

    - name: Hours
      description: Required access-token lifetime in hours. Must be greater than zero.
      type: int

    - name: RefreshTokenDays
      description: Required refresh-token cookie lifetime in days. Must be greater than zero.
      type: int

    - name: LocalhostApp
      description: Application audience used for `localhost`, `127.0.0.1`, and `::1` requests when host app validation is active.
      type: string?
      default: 'null'

    - name: Hosts
      description: Map of request host names to application audience names. Empty mappings disable host app validation.
      type: 'Dictionary<string, string>'
      default: '[]'
---

# AuthSettings

Represents the `Auth` configuration section consumed by ASP.NET JWT Auth. The package binds this record during [`AddJwtAuth`](../extensions/add-jwt-auth), and the JWT bearer setup reads the same section when it configures issuer, signing-key, lifetime, and audience validation.

Application code normally does not create `AuthSettings` manually. Register the package with `builder.Configuration`, then inject `IOptions<AuthSettings>` when token-issuing code needs the configured issuer, token lifetime, refresh-token lifetime, or host mapping. `Hosts` controls whether app-audience validation is active: when mappings exist, tokens must use one of the configured app audiences and protected requests must match the app resolved from the current host.

## Usage

::: tip
The JSON shape is documented on the [configuration page](../configuration). The example below shows how application services can consume the already-bound options.
:::

```csharp
using Microsoft.Extensions.Options;
using AlmightyShogun.AspNet.JwtAuth;

public sealed class TokenIssuer(IOptions<AuthSettings> options)
{
    private readonly AuthSettings _auth = options.Value;

    public DateTimeOffset GetAccessTokenExpiry()
        => DateTimeOffset.UtcNow.AddHours(_auth.Hours);
}
```

<FrontmatterDocs/>
