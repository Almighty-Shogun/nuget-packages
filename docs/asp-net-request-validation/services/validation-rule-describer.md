# ValidationRuleDescriber

Reports the validation rules declared on a request type as structured data, so an application can publish its own rules endpoint, generate client-side validation, or render form hints from the same declarations the server enforces. Application code depends on `IValidationRuleDescriber`.

::: danger
Exposing this on an unauthenticated endpoint publishes your request shapes, field names, and constraints, including any pattern or length limit that reveals something about the data. Treat it as internal API surface unless you have decided otherwise deliberately.
:::

## Describe

Returns the rules for each property that declares at least one attribute rule, keyed by the field name a client sees, which honours [`[JsonPropertyName]`](https://learn.microsoft.com/dotnet/api/system.text.json.serialization.jsonpropertynameattribute) where a property carries one. Properties with no validation attributes are omitted entirely rather than mapped to an empty list, and results are cached per request type, so repeated calls do not re-reflect.

Each description is read from the attribute metadata the rule factory builds from, so a described attribute rule cannot drift from the rule built for it, and the arguments come back in constructor order including the defaults the call site left unwritten. Rules declared in a [`Validator<TRequest>`](../fluent-validation) are enforced but not described, since a built rule keeps neither the name it was written under nor the arguments it was given, so a request using both describes as less than it enforces.

::: code-group

```csharp [RulesController.cs]
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AlmightyShogun.AspNet.RequestValidation;

[ApiController]
[Route("internal/rules")]
[Authorize(Policy = "internal")]
public sealed class RulesController(
    IValidationRuleDescriber describer
) : ControllerBase
{
    [HttpGet("signup")]
    public IActionResult GetSignupRules()
        => Ok(describer.Describe<SignupRequest>());
}
```

```csharp [SignupRequest.cs]
using System.Text.RegularExpressions;
using AlmightyShogun.AspNet.RequestValidation;

public sealed record SignupRequest
{
    [Regex("^[a-z0-9]+$", RegexOptions.None, "lowercase letters and digits only")]
    public string Username { get; init; } = "";
}
```

```json [Response.json]
{
    "username": [
        {
            "rule": "Regex",
            "arguments": ["^[a-z0-9]+$", 0, "lowercase letters and digits only", 1]
        }
    ]
}
```

:::

### Type signature

```csharp
public IReadOnlyDictionary<
    string, IReadOnlyList<ValidationRuleDescription>
> Describe<TRequest>() where TRequest : class;
```
