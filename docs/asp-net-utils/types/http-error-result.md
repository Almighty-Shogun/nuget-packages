# HttpErrorResult

Creates MVC `ObjectResult` values for standardized HTTP error responses. The helper exists for controller actions that manually construct an `HttpErrorResponse` and want the MVC result status code to match the response body.

Most applications do not need to call this type directly because `UseHttpErrorResponses`, `HttpErrorException`, and the package MVC filter cover the common error paths. Use it when an action already has a concrete `HttpErrorResponse` value and should return it through MVC.

## Usage

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Utils;

[ApiController]
[Route("users")]
public sealed class UsersController : ControllerBase
{
    [HttpGet("{id:int}")]
    public ObjectResult GetUser(int id)
    {
        var response = new HttpErrorResponse
        {
            Code = StatusCodes.Status404NotFound,
            Error = "not_found",
            ErrorDescription = $"User {id} was not found."
        };

        return HttpErrorResult.Create(response);
    }
}
```

## Create

Creates an `ObjectResult` with the supplied response as the body and `HttpErrorResponse.Code` as the MVC result status code.

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Utils;

var response = new HttpErrorResponse
{
    Code = StatusCodes.Status400BadRequest,
    Error = "bad_request",
    ErrorDescription = "The request is invalid."
};

ObjectResult result = HttpErrorResult.Create(response);
```

### Type signature

```csharp
public static ObjectResult Create(HttpErrorResponse response);
```
