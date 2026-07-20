# Date Rules

Date rules validate parseable dates, exact date formats, and date ordering against literal dates or other request fields. Attribute rules use `ComparisonTarget` for string targets; fluent rules use typed date values or property expressions.

Use `ComparisonTarget.Value` when an attribute target string is a literal date value. Use `ComparisonTarget.Field` when the target string names another request property. Nullable request DTO properties are usually safer for required dates because non-nullable value types receive a default value during model binding.

## Date

Requires the value to be a date or date/time value that can be parsed.

::: code-group

```csharp [Attribute.cs]
[Date]
```

```csharp [FluentRule.cs]
RuleFor(x => x.StartsAt)
    .Date();
```

:::

## DateFormat

Requires the value to be a date string matching the exact configured format.

::: code-group

```csharp [Attribute.cs]
[DateFormat(string format)]

[DateFormat("yyyy-MM-dd")]
```

```csharp [FluentRule.cs]
RuleFor(x => x.BirthDate)
    .DateFormat("yyyy-MM-dd");
```

:::

## After

Requires the value to be after a literal date or another request field.

::: code-group

```csharp [Attribute.cs]
[After(string target, ComparisonTarget targetType = ComparisonTarget.Value)]

[After("2026-01-01")]
[After("StartsAt", ComparisonTarget.Field)]
```

```csharp [FluentRule.cs]
RuleFor(x => x.EndsAt)
    .After(x => x.StartsAt);
```

:::

## AfterOrEqual

Requires the value to be after or equal to a literal date or another request field.

::: code-group

```csharp [Attribute.cs]
[AfterOrEqual(string target, ComparisonTarget targetType = ComparisonTarget.Value)]

[AfterOrEqual("2026-01-01")]
[AfterOrEqual("StartsAt", ComparisonTarget.Field)]
```

```csharp [FluentRule.cs]
RuleFor(x => x.EndsAt)
    .AfterOrEqual(x => x.StartsAt);
```

:::

## Before

Requires the value to be before a literal date or another request field.

::: code-group

```csharp [Attribute.cs]
[Before(string target, ComparisonTarget targetType = ComparisonTarget.Value)]

[Before("2026-12-31")]
[Before("EndsAt", ComparisonTarget.Field)]
```

```csharp [FluentRule.cs]
RuleFor(x => x.StartsAt)
    .Before(x => x.EndsAt);
```

:::

## BeforeOrEqual

Requires the value to be before or equal to a literal date or another request field.

::: code-group

```csharp [Attribute.cs]
[BeforeOrEqual(string target, ComparisonTarget targetType = ComparisonTarget.Value)]

[BeforeOrEqual("2026-12-31")]
[BeforeOrEqual("EndsAt", ComparisonTarget.Field)]
```

```csharp [FluentRule.cs]
RuleFor(x => x.StartsAt)
    .BeforeOrEqual(x => x.EndsAt);
```

:::

## DateEquals

Requires the value to equal a literal date or another request field.

::: code-group

```csharp [Attribute.cs]
[DateEquals(string target, ComparisonTarget targetType = ComparisonTarget.Value)]

[DateEquals("2026-07-20")]
[DateEquals("ExpectedDate", ComparisonTarget.Field)]
```

```csharp [FluentRule.cs]
RuleFor(x => x.DueDate)
    .DateEquals(x => x.ExpectedDate);
```

:::
