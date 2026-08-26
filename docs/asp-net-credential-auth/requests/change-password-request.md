---
fields:
    - name: CurrentPassword
      description: The password in force now. A wrong value raises `InvalidCredentialsException`, the same failure a wrong login password produces.
      type: string

    - name: NewPassword
      description: The replacement, at least 8 characters and subject to the `[PasswordSecure]` rule. Raises `PasswordReusedException` when it verifies against the password already stored.
      type: string

    - name: ConfirmPassword
      description: The new password typed again. Compared by the service, not during validation, so a mismatch arrives as `PasswordMismatchException`.
      type: string
---

# ChangePasswordRequest

What [`ChangePasswordAsync`](../services/auth-password-service#changepasswordasync) takes from a signed-in user. Validation checks shape and strength only; the three password comparisons are made by the service against the stored hash.

## Usage

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.JwtAuth;
using AlmightyShogun.AspNet.CredentialAuth;

public sealed class ChangePasswordController(IAuthPasswordService passwords) : ControllerBase
{
    [RequireRefreshToken]
    public async Task<IActionResult> Change(ChangePasswordRequest request)
    {
        Guid identifier = User.GetCurrentUserId();
        string refreshToken = Request.GetRefreshTokenCookie();

        await passwords.ChangePasswordAsync(identifier, request, refreshToken);

        return NoContent();
    }
}
```

<FrontmatterDocs/>

## Type signature

```csharp
public class ChangePasswordRequest;
```
