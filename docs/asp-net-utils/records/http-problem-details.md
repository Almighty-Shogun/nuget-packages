---
fields:
    - name: Type
      description: URI reference identifying the problem type. `about:blank` carries no meaning beyond the status code, which is what RFC 9457 prescribes when there is no type to point at.
      type: string
      default: about:blank

    - name: Title
      description: Short identifier for the problem type. Carries the package error code, so the machine-readable value survives the switch to problem details.
      type: string

    - name: Status
      description: The HTTP status code.
      type: int

    - name: Detail
      description: Human-readable explanation, resolved through the message resolver. Omitted from the body when null.
      type: string?

    - name: Instance
      description: Request path the problem occurred on. Omitted from the body when null.
      type: string?
---

# HttpProblemDetails

The RFC 9457 problem details body, written in place of [`HttpErrorResponse`](./http-error-response) when `UseProblemDetails` is enabled in [`HttpErrorSettings`](../configuration/http-error-settings). The response content type becomes `application/problem+json`.

Enabling it changes every error body the application returns, including the per-field validation response from `AlmightyShogun.AspNet.Validation`, which extends this record rather than defining a second shape.

## Usage

Nothing constructs it directly; [`IHttpErrorResponseWriter`](../services/http-error-response-writer) does. A client reads it:

```csharp
using System.Net.Http.Json;
using AlmightyShogun.AspNet.Utils;

HttpProblemDetails? problem = await response.Content
    .ReadFromJsonAsync<HttpProblemDetails>(cancellationToken);
```

With the ASP.NET Core defaults that produces:

```json
{
    "type": "about:blank",
    "title": "not_found",
    "status": 404,
    "detail": "The order could not be found",
    "instance": "/orders/12"
}
```

<FrontmatterDocs/>

## Type signature

```csharp
public record HttpProblemDetails
{
    public string Type { get; init; }
    public required string Title { get; init; }
    public required int Status { get; init; }
    public string? Detail { get; init; }
    public string? Instance { get; init; }
}
```
