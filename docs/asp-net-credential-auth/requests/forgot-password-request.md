# ForgotPasswordRequest

Represents the first step of a forgot-password flow. The request contains the email address that should receive a password reset token when a matching credential user exists.

Use this DTO with [`IAuthPasswordService.RequestForgotPasswordAsync`](../services/auth-password-service#requestforgotpasswordasync). The returned token is plain text and should be sent through application-owned mail logic; Credential Auth stores only the token hash.

## Usage

::: code-group

```csharp [ForgotPasswordController.cs]
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.CredentialAuth;

public sealed class ForgotPasswordController(
    IPasswordResetMailer mailer,
    IAuthPasswordService passwords) : ControllerBase
{
    public async Task<IActionResult> Start(ForgotPasswordRequest request)
    {
        string? ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        string token = await passwords.RequestForgotPasswordAsync(request, ipAddress);

        await mailer.SendAsync(request.Email, token);

        return NoContent();
    }
}
```

```csharp [IPasswordResetMailer.cs]
public interface IPasswordResetMailer
{
    Task SendAsync(string email, string token);
}
```

:::

## Type signature

```csharp
public class ForgotPasswordRequest
{
    public required string Email { get; set; }
}
```
