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

## Usage

Construct one directly only when returning it through [`HttpErrorResult`](../types/http-error-result). Everywhere else, [`IHttpErrorResponseWriter`](../services/http-error-response-writer) builds it.

```csharp
using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Core;

HttpErrorResponse response = new()
{
    Code = StatusCodes.Status409Conflict,
    Error = "order_already_shipped",
    ErrorDescription = messageResolver.Resolve("orders.already-shipped")
};
```

::: tip
Property names are camel-cased by the default ASP.NET Core serializer, so `ErrorDescription` appears as `errorDescription` on the wire.
:::

<FrontmatterDocs/>
