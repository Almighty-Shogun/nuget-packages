# ForgotPasswordTokenRequest

Represents a request that only checks whether a password reset token is still valid. Use it when a frontend wants to validate a token before showing a reset-password form.

The `Token` field includes [`PasswordResetToken`](../attributes/password-reset-token-attribute), which checks the submitted token against active stored token hashes. This request does not change the password and does not mark the token as used.

## Usage

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.CredentialAuth;

public sealed class PasswordTokenController : ControllerBase
{
    public IActionResult Check(ForgotPasswordTokenRequest request)
        => NoContent();
}
```

## Type signature

```csharp
public class ForgotPasswordTokenRequest
{
    public required string Token { get; set; }
}
```
