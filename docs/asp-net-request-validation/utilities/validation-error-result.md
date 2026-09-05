# ValidationErrorResult

Builds the standardized validation error result for a single field. The helper exists for controller actions that need to return the same response shape produced by automatic request validation, for a failure the rules cannot express, such as a uniqueness check against the database.

## Create

Creates an [`HttpErrorResult`](/asp-net-core/types/http-error-result) with status code `422`, the top-level `validation_error` identifier, and a single field error resolved from the supplied validation message key. Nothing here converts the field name, since this path has no property to read a serialization name from, so spell it the way the client sees it.

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
    public HttpErrorResult Create(InviteUserRequest request)
    {
        return ValidationErrorResult.Create(
            messageResolver,
            "email",
            "validation.unique"
        );
    }
}
```

### Type signature

```csharp
public static HttpErrorResult Create(
    IMessageResolver messageResolver,
    string field,
    string key,
    params object?[] parameters
);
```
