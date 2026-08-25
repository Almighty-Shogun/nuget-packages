---
fields:
    - name: Errors
      description: Field-level errors keyed by the request property name, each one a [`ValidationRuleError`](./validation-rule-error).
      type: 'IReadOnlyDictionary<string, ValidationRuleError>'
---

# ValidationErrorResponse

Represents the standardized validation error response body returned by ASP.NET Request Validation. It extends ASP.NET Core [`HttpErrorResponse`](/asp-net-core/records/http-error-response) with field-level validation errors grouped under `Errors`.

## Usage

```csharp
using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.RequestValidation;

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

<FrontmatterDocs/>

## Type signature

```csharp
public sealed record ValidationErrorResponse : HttpErrorResponse
{
    public required IReadOnlyDictionary<string, ValidationRuleError> Errors
    {
        get;
        init;
    }
}
```
