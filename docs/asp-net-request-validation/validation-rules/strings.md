# String Rules

String rules validate textual content and text containment. They do not replace type validation; use [Types and Files](./types-and-files) when the value must specifically be a string, then add these rules for content.

## Alpha

Requires the value to contain only letters.

::: code-group

```csharp [Attribute.cs]
[Alpha]
```

```csharp [FluentRule.cs]
RuleFor(x => x.FirstName)
    .Alpha();
```

:::

## AlphaNumeric

Requires the value to contain only letters and numbers.

::: code-group

```csharp [Attribute.cs]
[AlphaNumeric]
```

```csharp [FluentRule.cs]
RuleFor(x => x.ReferenceCode)
    .AlphaNumeric();
```

:::

## AlphaDash

Requires the value to contain only letters, numbers, dashes, and underscores. Use it for slugs, handles, and similar identifier-style text.

::: code-group

```csharp [Attribute.cs]
[AlphaDash]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Slug)
    .AlphaDash();
```

:::

## Ascii

Requires the value to contain only single-byte ASCII characters.

::: code-group

```csharp [Attribute.cs]
[Ascii]
```

```csharp [FluentRule.cs]
RuleFor(x => x.LegacyCode)
    .Ascii();
```

:::

## Lowercase

Requires the text value to already be lowercase.

::: code-group

```csharp [Attribute.cs]
[Lowercase]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Username)
    .Lowercase();
```

:::

## Uppercase

Requires the text value to already be uppercase.

::: code-group

```csharp [Attribute.cs]
[Uppercase]
```

```csharp [FluentRule.cs]
RuleFor(x => x.CountryCode)
    .Uppercase();
```

:::

## StartsWith

Requires the text value to start with one of the provided prefixes.

::: code-group

```csharp [Attribute.cs]
[StartsWith(params string[] values)]

[StartsWith("ORD-", "INV-")]
```

```csharp [FluentRule.cs]
RuleFor(x => x.OrderNumber)
    .StartsWith("ORD-", "INV-");
```

:::

## EndsWith

Requires the text value to end with one of the provided suffixes.

::: code-group

```csharp [Attribute.cs]
[EndsWith(params string[] values)]

[EndsWith(".jpg", ".png")]
```

```csharp [FluentRule.cs]
RuleFor(x => x.FileName)
    .EndsWith(".jpg", ".png");
```

:::

## Contains

Requires the text value or collection to contain one of the provided values.

::: code-group

```csharp [Attribute.cs]
[Contains(params string[] values)]

[Contains("admin", "editor")]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Roles)
    .Contains("admin", "editor");
```

:::

## DoesNotStartWith

Rejects text that starts with one of the provided prefixes.

::: code-group

```csharp [Attribute.cs]
[DoesNotStartWith(params string[] values)]

[DoesNotStartWith("admin", "system")]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Username)
    .DoesNotStartWith("admin", "system");
```

:::

## DoesNotEndWith

Rejects text that ends with one of the provided suffixes.

::: code-group

```csharp [Attribute.cs]
[DoesNotEndWith(params string[] values)]

[DoesNotEndWith(".tmp", ".bak")]
```

```csharp [FluentRule.cs]
RuleFor(x => x.FileName)
    .DoesNotEndWith(".tmp", ".bak");
```

:::

## DoesNotContain

Rejects text or collections containing one of the provided values.

::: code-group

```csharp [Attribute.cs]
[DoesNotContain(params string[] values)]

[DoesNotContain("password", "secret")]
```

```csharp [FluentRule.cs]
RuleFor(x => x.DisplayName)
    .DoesNotContain("password", "secret");
```

:::
