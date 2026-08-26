---
fields:
    - name: Email
      description: The address to send a reset link to, checked for a valid shape by `[Email]`. An address matching no account is not an error, so the response cannot be used to test which addresses are registered.
      type: string
---

# ForgotPasswordRequest

What [`RequestForgotPasswordAsync`](../services/auth-password-service#requestforgotpasswordasync) takes to start a reset. It returns the token in plain text for the application to email, or `null` when nothing matched.

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
        string? token = await passwords.RequestForgotPasswordAsync(request, ipAddress);

        if (token is not null)
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

<FrontmatterDocs/>

## Type signature

```csharp
public class ForgotPasswordRequest;
```
