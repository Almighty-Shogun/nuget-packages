# EmailVerificationPurpose

Says which flow an [`EmailVerificationToken`](./email-verification-token) was issued for, which is what stops one being redeemed on the other's endpoint. A `Registration` token carries the address the account already holds, and redeeming it stamps [`EmailVerifiedAt`](./auth-user) without touching the address. An `EmailChange` token carries an address the account asked to move to, and redeeming it writes that address onto the user.

## Usage

```csharp
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.AspNet.Auth.Credentials;

DateTimeOffset now = DateTimeOffset.UtcNow;

List<EmailVerificationToken> live = await database.EmailVerificationTokens
    .Where(token => token.UserId == user.Id && token.UsedAt == null)
    .Where(token => token.ExpiresAt > now)
    .ToListAsync();

bool awaitingSignupConfirmation = live
    .Any(token => token.Purpose == EmailVerificationPurpose.Registration);

string? pendingAddress = live
    .Where(token => token.Purpose == EmailVerificationPurpose.EmailChange)
    .Select(token => token.Email)
    .FirstOrDefault();
```

## Type signature

```csharp
public enum EmailVerificationPurpose
{
    Registration = 0,
    EmailChange = 1
}
```
