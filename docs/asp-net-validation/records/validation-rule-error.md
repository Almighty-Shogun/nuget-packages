# ValidationRuleError

Represents one field-level validation error inside [`ValidationErrorResponse`](./validation-error-response). The record contains the numeric status code, the validation message key, and the localized message returned by the configured message resolver.

Use this record when tests need to assert validation response contents or when application code manually builds a [`ValidationErrorResponse`](./validation-error-response).

## Usage

```csharp
using AlmightyShogun.AspNet.Validation;

var error = new ValidationRuleError
{
    Code = 422,
    Error = "validation.required.default",
    ErrorDescription = "This field is required."
};
```

## Type signature

```csharp
public sealed record ValidationRuleError
{
    public required long Code { get; init; }
    public required string Error { get; init; }
    public required string? ErrorDescription { get; init; }
}
```
