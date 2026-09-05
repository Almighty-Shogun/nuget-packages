---
fields:
    - name: Code
      description: A stable number derived from the validation message key, so a client can branch on it without matching English text, and adding rules never renumbers the existing ones.
      type: long
    - name: Error
      description: The machine-readable identifier derived from the same key, with every run of non-alphanumeric characters replaced by an underscore, such as `validation_required_default`.
      type: string
    - name: ErrorDescription
      description: The localized message produced by the configured message resolver, which falls back to the message key itself when no message file defines it.
      type: string?
---

# ValidationRuleError

Represents one field-level validation error inside [`ValidationErrorResponse`](./validation-error-response). It carries the stable numeric code, the machine-readable error identifier, and the message resolved into the request's language, which is the one field that is not safe to branch on.

Use this record when tests need to assert validation response contents or when application code manually builds a [`ValidationErrorResponse`](./validation-error-response).

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
