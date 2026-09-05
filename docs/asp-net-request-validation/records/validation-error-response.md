---
fields:
    - name: Errors
      description: One entry per field that failed, keyed by the field name a client sees and holding that field's first failure as a [`ValidationRuleError`](./validation-rule-error). A property renamed with `[JsonPropertyName]` is keyed under that name, and a model-binding failure inside a nested object keeps its full path, such as `billingAddress.street` or `items[0].name`.
      type: 'IReadOnlyDictionary<string, ValidationRuleError>'
---

# ValidationErrorResponse

Represents the standardized validation error response body returned by ASP.NET Request Validation. It extends ASP.NET Core [`HttpErrorResponse`](/asp-net-core/records/http-error-response) with field-level validation errors grouped under `Errors`, so the outer three fields are the error body every package shares and `Errors` is what this package adds. Every rule failure, model-binding failure, and unreadable body is answered with this shape.

## Usage

::: code-group

```csharp [ValidationErrorResponse.cs]
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
            Code = 790803925,
            Error = "validation_email",
            ErrorDescription = "This field must be a valid email address."
        }
    }
};
```

```json [Response.json]
{
    "code": 422,
    "error": "validation_error",
    "errorDescription": "The given data was invalid.",
    "errors": {
        "email": {
            "code": 790803925,
            "error": "validation_email",
            "errorDescription": "This field must be a valid email address."
        }
    }
}
```

:::

<FrontmatterDocs/>

## Type signature

```csharp
public sealed record ValidationErrorResponse : HttpErrorResponse
{
    public required IReadOnlyDictionary<string, ValidationRuleError> Errors { get; init; }
}
```
