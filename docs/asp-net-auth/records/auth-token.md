---
fields:
    - name: Token
      description: The encoded JWT to return to the client.
      type: string

    - name: ExpiresAt
      description: The absolute expiry, in UTC.
      type: DateTimeOffset
---

# AuthToken

A minted access token and when it expires, returned by [`IAuthTokenGenerator.Generate`](../services/auth-token-generator).

`ExpiresAt` is returned so a client can refresh proactively rather than waiting for a `401`. It is the same value as the token's `exp` claim, exposed so the caller does not have to decode the token to read it.

<FrontmatterDocs/>

## Type signature

```csharp
public sealed record AuthToken
{
    public required string Token { get; init; }
    public required DateTimeOffset ExpiresAt { get; init; }
}
```
