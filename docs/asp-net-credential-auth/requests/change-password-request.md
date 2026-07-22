# ChangePasswordRequest

Represents a password-change request for an already authenticated user. The current password is validated against the current user, the new password must satisfy the secure password rule, and the confirmation password must match the new password.

Use this DTO with [`IAuthPasswordService.ChangePasswordAsync`](../services/auth-password-service#changepasswordasync). `CurrentPassword` is validated by [`CurrentPassword`](../attributes/current-password-attribute), and `NewPassword` uses [`NotCurrentPassword`](../attributes/not-current-password-attribute), so password-change requests cannot reuse the currently stored password. The service still checks that the target user exists before saving the new password.

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
        int userId = User.GetCurrentUserId();
        string refreshToken = Request.GetRefreshTokenCookie();

        await passwords.ChangePasswordAsync(userId, request, refreshToken);

        return NoContent();
    }
}
```

## Type signature

```csharp
public class ChangePasswordRequest
{
    public required string CurrentPassword { get; set; }
    public required string NewPassword { get; set; }
    public required string ConfirmPassword { get; set; }
}
```
