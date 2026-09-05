---
fields:
    - name: IsValid
      description: Whether the rule passed.
      type: bool

    - name: Key
      description: The message key describing the failure, resolved from the [message files](../localization) when the response is written. Empty on a success.
      type: string

    - name: Parameters
      description: The values substituted into the resolved message template by position. Empty on a success.
      type: 'object?[]'
---

# ValidationRuleResult

Represents the result returned by a custom validation rule. Built-in rules use the same structure internally, but application code most commonly interacts with this record from [`ICustomValidationRule<TRequest, TProperty>`](../custom-rules). It is built through `Success` and `Failure` rather than by construction, so a pass cannot carry a failure message and a failure cannot carry none.

<FrontmatterDocs/>

## Success

Returns the one shared successful result, since a passing rule carries no message and every success is therefore identical.

```csharp
using AlmightyShogun.AspNet.RequestValidation;

ValidationRuleResult result = ValidationRuleResult.Success();
```

### Type signature

```csharp
public static ValidationRuleResult Success();
```

## Failure

Reports a failure by message key, leaving the wording to be resolved from the [message files](../localization) when the response is written. Parameters are substituted into the message template by position, so a message that takes none is reported with the key alone.

```csharp
using AlmightyShogun.AspNet.RequestValidation;

ValidationRuleResult result = ValidationRuleResult.Failure(
    "validation.min.string",
    8
);
```

### Type signature

```csharp
public static ValidationRuleResult Failure(
    string key,
    params object?[] parameters
);
```

## Type signature

```csharp
public sealed record ValidationRuleResult
{
    public bool IsValid { get; }
    public string Key { get; }
    public object?[] Parameters { get; }
}
```
