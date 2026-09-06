---
fields:
    - name: User
      description: The user whose password verified, loaded without their two-factor enrolment, so no secret rides along with it. Populated on both outcomes, so the account can be named without querying again. It is the database entity and serializes with the password hash, the surrogate key, and any loaded sessions, so map it to a DTO before returning it.
      type: TUser

    - name: Session
      description: The access token, the refresh token, and the user, as [`AuthSessionResult<TUser>`](./auth-session-result). Null when a second factor is owed, because nothing is issued at all in that case.
      type: 'AuthSessionResult<TUser>?'
      default: 'null'

    - name: Challenge
      description: The challenge in plain text, to hand back to the client and send to [`CompleteTwoFactorLoginAsync`](../services/auth-user-service#completetwofactorloginasync) with the code. This is the only place it appears in that form; only its hash is stored. Null when no second factor was owed.
      type: string?
      default: 'null'

    - name: RequiresTwoFactor
      description: Whether the sign-in is unfinished, which is what the caller branches on. True means `Session` is null and no token was minted. Computed from `Challenge`, not stored, and annotated so branching on it narrows the branch's own property to non-null.
      type: bool
---

# AuthLoginResult

What [`LoginAsync`](../services/auth-user-service#loginasync) returns: either the session a correct password opened, or the challenge the account's enabled second factor has to be presented against. Exactly one of `Session` and `Challenge` is populated, and `RequiresTwoFactor` says which.

::: warning
`Challenge` names a sign-in whose password has already verified, so it is a credential in its own right. Put it in the response body rather than in a URL, and never write it to a log.
:::

<FrontmatterDocs/>

## Type signature

```csharp
public sealed class AuthLoginResult<TUser> where TUser : AuthUser
{
    public required TUser User { get; init; }
    public AuthSessionResult<TUser>? Session { get; init; }
    public string? Challenge { get; init; }
    public bool RequiresTwoFactor { get; }
}
```
