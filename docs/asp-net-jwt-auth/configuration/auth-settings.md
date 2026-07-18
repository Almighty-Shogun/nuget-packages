---
fields:
    - name: Issuer
      description: Expected JWT issuer value.
      type: string

    - name: Secret
      description: Symmetric signing secret used to validate JWT access tokens.
      type: string

    - name: Hours
      description: Access-token lifetime in hours.
      type: int

    - name: RefreshTokenDays
      description: Refresh-token cookie lifetime in days.
      type: int

    - name: LocalhostApp
      description: Application audience used for `localhost` requests during development.
      type: string?
      default: 'null'

    - name: Hosts
      description: Map of request host names to application audience names.
      type: 'Dictionary<string, string>'
      default: '[]'
---

# AuthSettings

Represents the `Auth` configuration section consumed by ASP.NET JWT Auth. The package binds this record during `AddApiAuth`, and the JWT bearer setup reads the same section when it configures token validation.

Application code normally does not create `AuthSettings` manually. Register the package with `builder.Configuration`, then inject `IOptions<AuthSettings>` when token-issuing code needs the configured issuer, token lifetime, refresh-token lifetime, or host mapping.

## Usage

The JSON shape is documented on the [configuration page](../configuration). The example below shows how application services can consume the already-bound options.

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
