---
fields:
    - name: Code
      description: The HTTP status code returned by the response.
      type: int

    - name: Error
      description: Stable machine-readable identifier for the failure, such as `invalid_credentials`.
      type: string

    - name: ErrorDescription
      description: Human-readable description, resolved for the request's language.
      type: string?
      default: 'null'
---

# HttpErrorResponse

The standardized error body returned by every package in this repository. `ErrorDescription` is the only optional field, and holds the unresolved message key when no message file defines it.

```json
{
    "code": 401,
    "error": "invalid_credentials",
    "errorDescription": "Authentication failed"
}
```

<FrontmatterDocs/>

## Type signature

```csharp
public record HttpErrorResponse
{
    public required int Code { get; init; }
    public required string Error { get; init; }
    public string? ErrorDescription { get; init; }
}
```
