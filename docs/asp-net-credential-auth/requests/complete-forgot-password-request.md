# CompleteForgotPasswordRequest

Represents the final step of a forgot-password flow. The request contains the reset token, the new password, and a confirmation password.

Use this DTO with [`IAuthPasswordService.CompleteForgotPasswordAsync`](../services/auth-password-service#completeforgotpasswordasync). The token must still be active, the new password must satisfy the secure password rule, and the confirmation must match before the service updates the stored password.

The `NewPassword` field uses [`NotCurrentPassword`](../attributes/not-current-password-attribute). During reset validation, the rule resolves the active password reset token and rejects the request when the submitted new password still matches the token owner's stored password.

## Usage

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.CredentialAuth;

public sealed class ResetPasswordController(IAuthPasswordService passwords) : ControllerBase
{
    public async Task<IActionResult> Complete(CompleteForgotPasswordRequest request)
    {
        await passwords.CompleteForgotPasswordAsync(request);

        return NoContent();
    }
}
```

## Type signature

```csharp
public class CompleteForgotPasswordRequest
{
    public required string Token { get; set; }
    public required string NewPassword { get; set; }
    public required string ConfirmPassword { get; set; }
}
```
