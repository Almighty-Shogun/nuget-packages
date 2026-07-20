# Presence Rules

Presence rules control whether a field must exist, contain a value, be present even when empty, be missing, or be prohibited. They run before normal validation rules so missing or disallowed fields produce clear validation errors instead of later format, size, or comparison failures.

Use these rules for unconditional field state checks. Use [Conditional Presence](./conditional-presence) when the field state depends on another request field.

## Required

Requires the field to be present and contain a non-empty value. It fails for missing values, `null`, empty text, empty collections, and empty uploaded files.

::: code-group

```csharp [Attribute.cs]
[Required]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Email)
    .Required();
```

:::

## Filled

Requires the field to contain a value when the client sends it. Missing values are allowed, but present empty values fail validation.

::: code-group

```csharp [Attribute.cs]
[Filled]
```

```csharp [FluentRule.cs]
RuleFor(x => x.DisplayName)
    .Filled();
```

:::

## Present

Requires the field to exist in the request even when the value is allowed to be empty. Use it when the API must distinguish omitted fields from explicitly provided empty values.

::: code-group

```csharp [Attribute.cs]
[Present]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Bio)
    .Present();
```

:::

## Missing

Requires the field to be absent from the request. Use it for server-controlled values that clients must never send.

::: code-group

```csharp [Attribute.cs]
[Missing]
```

```csharp [FluentRule.cs]
RuleFor(x => x.InternalRole)
    .Missing();
```

:::

## Prohibited

Rejects the field when the client sends it with a non-empty value. Empty or missing values are allowed.

::: code-group

```csharp [Attribute.cs]
[Prohibited]
```

```csharp [FluentRule.cs]
RuleFor(x => x.LegacyPassword)
    .Prohibited();
```

:::
