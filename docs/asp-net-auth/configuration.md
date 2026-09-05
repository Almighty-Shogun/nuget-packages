---
fields:
    -   name: AuthSettings
        description: The `Auth` section, bound by [`AddAuth`](./extensions/add-auth). Required, and validated while the host starts, so a short secret or a missing audience stops the application there.
        fields:
            -   name: Issuer
                description: The issuer written into minted tokens and required of incoming ones.
                type: string

            -   name: Secret
                description: The symmetric signing secret. Must be at least 32 characters, which covers the 32 bytes HMAC-SHA256 requires; a shorter one fails validation at startup rather than at the first request.
                type: string

            -   name: AccessTokenMinutes
                description: How long a minted access token stays valid, in minutes. Kept short, because an access token cannot be revoked once issued.
                type: int
                default: '60'

            -   name: RefreshTokenDays
                description: How long a refresh token, and the cookie carrying it, stays valid, in days. This is how long a returning user stays signed in without re-entering credentials.
                type: int
                default: '30'

            -   name: ClockSkewSeconds
                description: Tolerance applied when checking expiry, in seconds, absorbing small clock differences between the machine that minted a token and the one validating it.
                type: int
                default: '30'

            -   name: DefaultApp
                description: The audience used when no host mapping applies. Required when `Hosts` is empty, since a token still needs an audience to be validated against.
                type: string?
                default: 'null'

            -   name: LocalhostApp
                description: The audience used for requests arriving from plain localhost in development. Without it, a local request is refused like any unmapped host.
                type: string?
                default: 'null'

            -   name: Hosts
                description: Request host to audience mapping. A non-empty mapping turns on host-based scoping, so a token's audience must match the app resolved from the request host.
                type: 'IReadOnlyDictionary<string, string>'
                default: '{}'

            -   name: SameSite
                description: The `SameSite` mode applied to the refresh token cookie. `None` requires a secure connection, so a browser drops the cookie over plain HTTP.
                type: SameSiteMode
                default: Lax
---

# Configuration

The `Auth` section is required, because `Issuer` and `Secret` have no defaults. Durations are plain integers in the unit each name carries,
and the section is validated while the host starts rather than at the first request.

```json
{
    "Auth": {
        "Issuer": "https://auth.example.com",
        "Secret": "replace-with-a-secret-of-at-least-32-bytes",
        "AccessTokenMinutes": 60,
        "RefreshTokenDays": 30,
        "ClockSkewSeconds": 30,
        "DefaultApp": "api",
        "LocalhostApp": "localhost",
        "SameSite": "Lax",
        "Hosts": {
            "api.example.com": "api",
            "admin.example.com": "admin"
        }
    }
}
```

::: warning
`Hosts` decides whether app scoping is on at all. Leave it empty and every token is validated against `DefaultApp`, which then becomes
required; fill it in and a token's audience must match the app the request host resolves to.
:::

<FrontmatterDocs/>
