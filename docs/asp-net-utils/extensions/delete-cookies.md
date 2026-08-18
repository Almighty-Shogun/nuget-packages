# DeleteCookies

Deletes one or more cookies by name, emitting an expired `Set-Cookie` for each. Blank and whitespace-only names are ignored, so a name read from configuration that turns out to be empty does not throw. The expiry is scoped to the root path and the current host, so a cookie written with a different path or domain is a different cookie to the browser and survives this call.

## Usage

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.Utils;

[ApiController]
[Route("sessions")]
public sealed class SessionsController : ControllerBase
{
    [HttpDelete]
    public IActionResult SignOut()
    {
        Response.DeleteCookies("access_token", "refresh_token");

        return NoContent();
    }
}
```

## Type signature

```csharp
public void DeleteCookies(params string[] cookieNames);
```
