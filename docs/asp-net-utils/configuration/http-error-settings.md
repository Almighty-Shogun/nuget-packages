# HttpErrorSettings

Controls how handled errors are written and logged. Bound from the optional `HttpErrors` section by [`AddHttpErrorResponseWriter`](../extensions/add-http-error-response-writer).

Turning on `UseProblemDetails` changes every error body the application returns, including the validation response from `AlmightyShogun.AspNet.Validation`.

## Usage

```csharp
using AlmightyShogun.AspNet.Utils;
using Microsoft.Extensions.Options;

public sealed class ErrorFormatReporter(
    IOptions<HttpErrorSettings> errorOptions
)
{
    public bool UsesProblemDetails()
        => errorOptions.Value.UseProblemDetails;
}
```
