---
fields:
    - name: Secret
      description: The Base32 shared secret, for the user to type into an authenticator that cannot scan a code. Shown once at enrolment and never retrievable again.
      type: string

    - name: Uri
      description: The `otpauth://totp/` URI carrying the same secret plus the issuer, account, digit count, and period. Render it as a QR code.
      type: string
---

# AuthTwoFactorResult

What [`BeginEnrolmentAsync`](../services/auth-two-factor-service#beginenrolmentasync) returns: the new shared secret in the two forms an authenticator app accepts.

::: warning
Both values are the secret itself. Return them only to the authenticated user enrolling, over the response to their own request, and never write them to a log.
:::

## Usage

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.Auth;
using Microsoft.AspNetCore.Authorization;
using AlmightyShogun.AspNet.CredentialAuth;

[Authorize]
[HttpPost("two-factor/begin")]
public async Task<ActionResult<AuthTwoFactorResult>> Begin()
    => Ok(await twoFactor.BeginEnrolmentAsync(User.GetCurrentUserId(), "Example"));
```

<FrontmatterDocs/>

## Type signature

```csharp
public sealed record AuthTwoFactorResult(string Secret, string Uri);
```
