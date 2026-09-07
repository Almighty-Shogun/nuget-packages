# AuthEmailService

Owns the address on an account: it issues and redeems the tokens that prove one belongs to the user holding it, and writes a new address once the user has proved it. Application code depends on `IAuthEmailService`, which is not generic: it works from the public identifier or the token rather than from a user entity.

Nothing here sends mail: a request method returns the token in plain text for the application to email, and redemption records the proof on [`EmailVerifiedAt`](../types/auth-user) without ever acting on it, so whether an unverified account may sign in stays your own check.

## RequestVerificationAsync

Issues a token for the address the user already holds and returns it in plain text for the application to email. Only its hash is stored, so this return value is the single opportunity to send it. Issuing for an account that is already verified is allowed, so offering a resend needs no state check first.

The new token retires the unspent registration tokens that user held, which stops an older link working even though it confirmed the same address. Retirement and issue share one transaction, so requests made one after another leave a single live link; two arriving at once can leave both, since no index enforces the limit. The token expires after [`EmailVerificationMinutes`](../configuration).

Throws [`InvalidCredentialsException`](../exceptions) when the identifier matches no account.

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.Auth;
using AlmightyShogun.AspNet.Auth.Credentials;

[ApiController]
[Route("auth/email")]
public sealed class EmailController(
    IAuthEmailService emails,
    VerificationMailer mailer
) : ControllerBase
{
    [HttpPost("verify")]
    public async Task<IActionResult> Verify()
    {
        Guid identifier = User.GetCurrentUserId();

        string token = await emails.RequestVerificationAsync(identifier);

        await mailer.SendAsync(identifier, token);

        return NoContent();
    }
}
```

### Type signature

```csharp
public Task<string> RequestVerificationAsync(
    Guid identifier,
    CancellationToken cancellationToken = default
);
```

## RequestEmailChangeAsync

Issues a token for an address the user asked to move to and returns it in plain text. The address is stored on the token and written onto the user only when the token is redeemed, so nothing about the account changes here. As with a registration token, the new one retires the user's unspent change tokens, so requests made one after another leave the newest link as the only one that works.

Whatever an application demands before allowing a change, a current password, a second factor, or a cooling-off period, is checked by the caller beforehand: this method receives an address and trusts the request was already allowed. Asking for the address the account already holds is accepted rather than refused.

Throws [`InvalidCredentialsException`](../exceptions) when the identifier matches no account, and [`EmailTakenException`](../exceptions) when another account already holds the address, compared under the database's collation.

::: warning
Email this token to `newEmail` rather than to whatever address the account holds now. The token exists to prove that address is reachable, and a link sent elsewhere moves the account to an address nobody confirmed.
:::

```csharp
using AlmightyShogun.AspNet.Auth.Credentials;

string token = await emails.RequestEmailChangeAsync(
    identifier,
    request.NewEmail
);

await mailer.SendAsync(request.NewEmail, token);
```

### Type signature

```csharp
public Task<string> RequestEmailChangeAsync(
    Guid identifier,
    string newEmail,
    CancellationToken cancellationToken = default
);
```

## CompleteVerificationAsync

Redeems the registration token carried by a [`CompleteEmailVerificationRequest`](../requests/complete-email-verification-request) and stamps the user as verified. The token identifies the user, so no signed-in caller is needed. The address is left alone, since for this purpose the token confirms the one the user already holds.

Spending the token and stamping the user happen in one transaction, so a token is never consumed without its effect landing. The token, the user, and the address check are read before that transaction opens, so the only work inside it is those two updates. Redemption is not idempotent: a second click on the same link is refused the way an expired one is.

The address the token names has to still be the account's, compared under the database's collation, so a link issued before the address moved cannot stamp the account as having proved the address it moved to.

Throws [`InvalidEmailVerificationTokenException`](../exceptions) when the token is unknown, already spent, expired, was issued for a change of address, or names an address the account no longer holds, and also when a concurrent request spends it first, since the token is claimed with a guarded update rather than on the strength of the read that found it.

A write to the token's row or the user's, such as another sign-in rehashing that password or a request issuing a fresh link, can commit while the redemption is open, which makes it lose and start over. It is retried a bounded number of times and then throws [`ConcurrentSessionUpdateException`](../exceptions), leaving the token unspent and the link still usable.

```csharp
using AlmightyShogun.AspNet.Auth.Credentials;

await emails.CompleteVerificationAsync(request);
```

### Type signature

```csharp
public Task CompleteVerificationAsync(
    CompleteEmailVerificationRequest request,
    CancellationToken cancellationToken = default
);
```

## CompleteEmailChangeAsync

Redeems a change token, moves the user to the address it carries, and stamps them as verified, since the address they now hold is the one they just proved. Passing the caller's own refresh token as `currentRefreshToken` leaves that one session alive; passing nothing ends every session. The other sessions go whenever a change is redeemed, since one can move the identifier the account signs in with, and a change writing the address already held is not special-cased.

The user's unspent registration links are retired in the same transaction, so none of them is still redeemable afterwards, including one naming the address the account has just moved to.

Throws [`InvalidEmailVerificationTokenException`](../exceptions) on a token that is unknown, spent, expired, or was issued for a registration, and [`EmailTakenException`](../exceptions) when another account claimed the address between the request and the redemption. An address claimed between that check and the write fails at the unique index instead. The address is written by a statement rather than by a save, so that failure arrives as the provider's own `DbException` with nothing wrapped around it.

A refresh on one of the user's other sessions can commit while the redemption is open, which makes it lose and start over. It is retried a bounded number of times and then throws [`ConcurrentSessionUpdateException`](../exceptions), leaving the token unspent and the link still usable.

```csharp
using AlmightyShogun.AspNet.Auth;
using AlmightyShogun.AspNet.Auth.Credentials;

string currentRefreshToken = Request.GetRefreshTokenCookie();

await emails.CompleteEmailChangeAsync(
    request,
    currentRefreshToken
);
```

### Type signature

```csharp
public Task CompleteEmailChangeAsync(
    CompleteEmailVerificationRequest request,
    string? currentRefreshToken = null,
    CancellationToken cancellationToken = default
);
```
