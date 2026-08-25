# ValidationErrorResult

Creates MVC `ObjectResult` values for standardized validation error responses. The helper exists for controller actions that need to manually return the same response shape produced by automatic request validation.

## Usage

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.Core;
using AlmightyShogun.AspNet.Localization;
using AlmightyShogun.AspNet.RequestValidation;

[ApiController]
[Route("invites")]
public sealed class InvitesController(
    IMessageResolver messageResolver
) : ControllerBase
{
    [HttpPost]
    public ObjectResult Create(InviteUserRequest request)
    {
        return ValidationErrorResult.Create(
            messageResolver,
            nameof(request.Email),
            "validation.unique"
        );
    }
}
```

## Create

Creates an `ObjectResult` with status code `422`, the top-level `validation_error` identifier, and a single field error resolved from the supplied validation message key.

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.Core;
using AlmightyShogun.AspNet.Localization;
using AlmightyShogun.AspNet.RequestValidation;

ObjectResult result = ValidationErrorResult.Create(
    messageResolver,
    "email",
    "validation.unique"
);
```

### Type signature

```csharp
public static ObjectResult Create(
    IMessageResolver messageResolver,
    string field,
    string key,
    params object?[] parameters
);
```
