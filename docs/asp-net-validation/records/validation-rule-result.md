# ValidationRuleResult

Represents the result returned by a custom validation rule. Built-in rules use the same structure internally, but application code most commonly interacts with this record from [`ICustomValidationRule<TRequest, TProperty>`](../custom-rules).

Return `Success` when the value is valid. Return `Failure` with a validation message key when the value is invalid. Optional parameters are passed to the message resolver and replace placeholders such as `{0}` in the localized message.

## Usage

```csharp
using AlmightyShogun.AspNet.Validation;

return isUnique
    ? ValidationRuleResult.Success()
    : ValidationRuleResult.Failure("validation.unique");
```

## Success

Creates a reusable successful validation result.

```csharp
using AlmightyShogun.AspNet.Validation;

ValidationRuleResult result = ValidationRuleResult.Success();
```

### Type signature

```csharp
public static ValidationRuleResult Success();
```

## Failure

Creates a failed validation result with a message key and optional message parameters.

```csharp
using AlmightyShogun.AspNet.Validation;

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

public sealed record ValidationRuleResult(
    bool IsValid,
    string Key,
    object?[] Parameters
);
```
