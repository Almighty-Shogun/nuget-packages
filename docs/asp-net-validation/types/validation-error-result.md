# ValidationErrorResult

Creates MVC `ObjectResult` values for standardized validation error responses. The helper exists for controller actions that need to manually return the same response shape produced by automatic request validation.

Most applications do not need to call this type directly because [`AddAspNetValidation`](../extensions/add-asp-net-validation) and [`UseAspNetValidation`](../extensions/use-asp-net-validation) cover model-state failures, endpoint validation, invalid request bodies, and [`ValidationException`](./validation-exception). Use it when an action already knows the exact field and validation message key to return.

## Usage

```csharp
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.Utils;
using AlmightyShogun.AspNet.Validation;

[ApiController]
[Route("invites")]
public sealed class InvitesController(IMessageResolver messageResolver) : ControllerBase
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
using AlmightyShogun.AspNet.Utils;
using AlmightyShogun.AspNet.Validation;

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
