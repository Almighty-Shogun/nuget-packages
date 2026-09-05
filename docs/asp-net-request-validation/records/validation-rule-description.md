---
fields:
    - name: Rule
      description: The rule name, taken from the attribute without its `Attribute` suffix, such as `Min`.
      type: string

    - name: Arguments
      description: The values the rule was declared with, in constructor order, including unwritten defaults.
      type: 'IReadOnlyList<object?>'
---

# ValidationRuleDescription

One validation rule declared on a request property, produced by [`IValidationRuleDescriber`](../services/validation-rule-describer).

`Rule` matches the attribute name without its suffix, so `MinAttribute` is reported as `Min`. That is the same name used in the [rule catalogue](../validation-rules/presence).

<FrontmatterDocs/>

## Type signature

```csharp
public sealed record ValidationRuleDescription
{
    public required string Rule { get; init; }
    public required IReadOnlyList<object?> Arguments { get; init; }
}
```
