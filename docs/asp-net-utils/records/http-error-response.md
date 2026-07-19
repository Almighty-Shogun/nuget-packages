# HttpErrorResponse

Represents the standardized HTTP error response body written by the package middleware, exception handler, MVC filter, and manual MVC result helper. The response contains the numeric status code, a stable machine-readable error identifier, and an optional localized description.

Use this record when application code needs to return the same error shape manually, inspect a generated error response in tests, or integrate custom MVC results with `HttpErrorResult`.

## Usage

```csharp
using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Utils;

var response = new HttpErrorResponse
{
    Code = StatusCodes.Status404NotFound,
    Error = "not_found",
    ErrorDescription = "The requested resource was not found."
};
```

## Type signature

```csharp
public record HttpErrorResponse
{
    public required int Code { get; init; }
    public required string Error { get; init; }
    public required string? ErrorDescription { get; init; }
}
```
