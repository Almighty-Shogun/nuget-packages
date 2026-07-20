# ValidationErrorResponse

Represents the standardized validation error response body returned by ASP.NET Validation. It extends ASP.NET Utils [`HttpErrorResponse`](/asp-net-utils/records/http-error-response) with field-level validation errors grouped under `Errors`.

The package writes this record for invalid model state, minimal API validation failures, [`ValidationException`](../types/validation-exception), and manual results created through [`ValidationErrorResult`](../types/validation-error-result). API clients can rely on `Code` for the HTTP status, `Error` for the top-level machine-readable error, `ErrorDescription` for the localized response description, and `Errors` for per-field validation failures.

## Usage

```csharp
using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Validation;

var response = new ValidationErrorResponse
{
    Code = StatusCodes.Status422UnprocessableEntity,
    Error = "validation_error",
    ErrorDescription = "The given data was invalid.",
    Errors = new Dictionary<string, ValidationRuleError>
    {
        ["email"] = new()
        {
            Code = 422,
            Error = "validation.email",
            ErrorDescription = "This field must be a valid email address."
        }
    }
};
```

## Type signature

```csharp
public sealed record ValidationErrorResponse : HttpErrorResponse
{
    public required IReadOnlyDictionary<string, ValidationRuleError> Errors { get; init; }
}
```
