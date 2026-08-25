# ValidationRuleDescriber

Reports the validation rules declared on a request type as structured data, so an application can publish its own rules endpoint, generate client-side validation, or render form hints from the same declarations the server enforces.

Application code depends on `IValidationRuleDescriber`.

The description is read from the validation attributes themselves, the same input the rule factory builds from, so a description cannot drift from what is actually validated.

::: danger
Exposing this on an unauthenticated endpoint publishes your request shapes, field names, and constraints, including any pattern or length limit that reveals something about the data. Treat it as internal API surface unless you have decided otherwise deliberately.
:::

## Usage

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
using AlmightyShogun.AspNet.RequestValidation;

public sealed record SignupRequest
{
    [Regex("^[a-z0-9]+$", RegexOptions.None, "lowercase letters and digits only")]
    public string Username { get; init; } = "";
}
```

```json [Response.json]
{
    "Username": [
        {
            "rule": "Regex",
            "arguments": ["^[a-z0-9]+$", 0, "lowercase letters and digits only", 1]
        }
    ]
}
```

:::

## Describe

Returns the rules for each property that declares at least one, keyed by property name. Properties with no validation attributes are omitted entirely rather than mapped to an empty list.

Results are cached per request type, so repeated calls do not re-reflect.

The request type is a type argument rather than a `Type`, so a caller cannot ask for something that was never a request.

```csharp
using AlmightyShogun.AspNet.RequestValidation;

IReadOnlyDictionary<
    string,
    IReadOnlyList<ValidationRuleDescription>
> rules = describer.Describe<SignupRequest>();
```

### Type signature

```csharp
IReadOnlyDictionary<
    string, IReadOnlyList<ValidationRuleDescription>
> Describe<TRequest>() where TRequest : class;
```
