---
fields:
    - name: Code
      description: HTTP status code the error is reported under.
      type: long
    - name: Error
      description: The validation message key, such as `validation.required.default`.
      type: string
    - name: ErrorDescription
      description: The localized message produced by the configured message resolver.
      type: string?
---

# ValidationRuleError

Represents one field-level validation error inside [`ValidationErrorResponse`](./validation-error-response). The record contains the numeric status code, the validation message key, and the localized message returned by the configured message resolver.

Use this record when tests need to assert validation response contents or when application code manually builds a [`ValidationErrorResponse`](./validation-error-response).

## Usage

```csharp
using AlmightyShogun.AspNet.RequestValidation;

var error = new ValidationRuleError
{
    Code = 422,
    Error = "validation.required.default",
    ErrorDescription = "This field is required."
};
```

<FrontmatterDocs/>

## Type signature

```csharp
public sealed record ValidationRuleError
{
    public required long Code { get; init; }
    public required string Error { get; init; }
    public required string? ErrorDescription { get; init; }
}
```
