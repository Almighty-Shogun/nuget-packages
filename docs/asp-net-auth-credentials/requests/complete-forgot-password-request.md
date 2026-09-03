---
fields:
    - name: Token
      description: The token from the reset link. It identifies the user on its own, so nothing else about the account is submitted with it.
      type: string

    - name: NewPassword
      description: The replacement, at least 8 characters and subject to the `[PasswordSecure]` rule. Raises `PasswordReusedException` when it verifies against the password already stored.
      type: string

    - name: ConfirmPassword
      description: The new password typed again. Compared by the service, not during validation, so a mismatch arrives as `PasswordMismatchException`.
      type: string
---

# CompleteForgotPasswordRequest

What [`CompleteForgotPasswordAsync`](../services/auth-password-service#completeforgotpasswordasync) takes to finish a reset. No signed-in caller is needed, because the token is what proves who is asking.

## Usage

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.Auth.Credentials;

public sealed class ResetPasswordController(IAuthPasswordService passwords) : ControllerBase
{
    public async Task<IActionResult> Complete(CompleteForgotPasswordRequest request)
    {
        await passwords.CompleteForgotPasswordAsync(request);

        return NoContent();
    }
}
```

<FrontmatterDocs/>

## Type signature

```csharp
public class CompleteForgotPasswordRequest;
```
