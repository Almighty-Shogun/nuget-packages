# Composition Rules

Composition rules combine multiple validation paths for the same field. Use them when a value is valid if it satisfies one of several rule sets, such as accepting either an email address or a username.

## AnyOf

Requires at least one configured rule set to pass. Each callback receives an isolated rule builder for the same field, and the field is valid when one of those builders completes without a rule failure. This rule is fluent-only because attributes cannot express nested rule groups cleanly.

::: code-group

```csharp [FluentRule.cs]
RuleFor(x => x.Login)
    .AnyOf(
        rules => rules.Email(),
        rules => rules.AlphaNumeric().Min(3).Max(32)
    );
```

:::

There is no attribute equivalent for `AnyOf`. An `AnyOf<TRule>` attribute would only be another name for `[CustomRule<TRule>]`, because C# attributes cannot hold nested fluent rule builders or call other validation attributes as executable rules. Use fluent `AnyOf` when composing built-in rules, and use `[CustomRule<TRule>]` when attribute-based code needs custom branching logic.
