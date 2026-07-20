# ValidationException

Represents a manual validation failure for application code that detects invalid input outside of a request attribute or fluent rule. The validation middleware and exception handler convert this exception into the same [`ValidationErrorResponse`](../records/validation-error-response) used by automatic validation.

Use this type when a controller, endpoint, service, or command needs to reject a specific request field after performing logic that cannot be expressed as a reusable rule. The `field` value is normalized to camel case in the response, and `key` is resolved through the message files described on the [Localization](../localization) page.

## Usage

```csharp
using AlmightyShogun.AspNet.Validation;

public sealed class InviteService(IInviteRepository invites)
{
    public async Task InviteAsync(InviteUserRequest request, CancellationToken cancellationToken)
    {
        bool alreadyInvited = await invites.ExistsAsync(request.Email, cancellationToken);

        if (alreadyInvited)
            throw new ValidationException("Email", "validation.unique");
    }
}
```

## Type signature

```csharp
public class ValidationException : Exception
{
    public ValidationException(
        string field,
        string key,
        params object?[] parameters
    );
}
```
