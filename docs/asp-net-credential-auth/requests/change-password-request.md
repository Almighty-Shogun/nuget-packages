# ChangePasswordRequest

Represents a password-change request for an already authenticated user. The current password is validated against the current user, the new password must satisfy the secure password rule, and the confirmation password must match the new password.

Use this DTO with [`IAuthPasswordService.ChangePasswordAsync`](../services/auth-password-service#changepasswordasync). The `NewPassword` field uses [`NotCurrentPassword`](../attributes/not-current-password-attribute), so password-change requests cannot reuse the currently stored password. The service verifies both the current password and the new-password reuse rule again before saving, so direct service calls cannot bypass the request validation attributes.

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
