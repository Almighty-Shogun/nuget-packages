# HttpErrorException

Represents an exception that should be converted into a standardized HTTP error response. The exception carries an HTTP status code and can optionally carry a message key plus formatting parameters for the response description.

Throw this exception from application code when the request should stop with a specific HTTP error and the response body should still use the package's normal error format. The `message` argument is treated as a message key, not as a final display string. If the key cannot be resolved from the configured message files, the key itself is returned as the fallback description.

## Usage

::: code-group

```csharp [UsersController.cs]
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Utils;

[ApiController]
[Route("users")]
public sealed class UsersController : ControllerBase
{
    [HttpGet("{id:int}")]
    public ActionResult GetUser(int id)
    {
        throw new HttpErrorException(
            StatusCodes.Status404NotFound,
            "users.not-found",
            id
        );
    }
}
```

```json [messages/en/users.json]
{
    "not-found": "User {0} was not found."
}
```

:::

## Type signature

```csharp
public class HttpErrorException : Exception
{
    public int StatusCode { get; }

    public HttpErrorException(
        int statusCode,
        string? message = null,
        params object?[] parameters
    );
}
```
