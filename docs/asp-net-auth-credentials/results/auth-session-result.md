---
fields:
    - name: AccessToken
      description: The signed JWT to return to the client. Its lifetime comes from `AccessTokenMinutes` in the JWT package's configuration.
      type: string

    - name: RefreshToken
      description: The refresh token in plain text, the only copy that will ever exist; only its hash is stored. Put it in the refresh-token cookie rather than the response body.
      type: string

    - name: User
      description: The authenticated user, tracked by the context. It is the database entity and serializes with the password hash, the surrogate key, and any loaded sessions, so map it to a DTO before returning it.
      type: TUser
---

# AuthSessionResult

What every flow that establishes a session returns: [`LoginAsync`](../services/auth-user-service#loginasync), [`RegisterAsync`](../services/auth-user-service#registerasync), and [`RefreshSessionAsync`](../services/auth-session-service#refreshsessionasync).

::: danger
`User` is the database entity. Never return it from an endpoint: it serializes with the password hash, the surrogate key, and any loaded sessions. Map it to a DTO that exposes only the fields the client needs.
:::

<FrontmatterDocs/>

## Type signature

```csharp
public sealed class AuthSessionResult<TUser> where TUser : AuthUser
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
    public required TUser User { get; init; }
}
```
