# Number Rules

Number rules validate numeric values, comparable sizes, digit counts, and numeric relationships. Comparable size rules adapt to the value type: strings use character count, arrays use item count, files use kilobytes, and numeric values use the number itself.

## Numeric

Requires the value to be numeric.

::: code-group

```csharp [Attribute.cs]
[Numeric]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Amount)
    .Numeric();
```

:::

## Integer

Requires the value to be an integer.

::: code-group

```csharp [Attribute.cs]
[Integer]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Quantity)
    .Integer();
```

:::

## Min

Requires the value to be at least the provided number. For strings, collections, and files, the number is interpreted as length, count, or kilobytes.

::: code-group

```csharp [Attribute.cs]
[Min(double value)]

[Min(3)]
```

```csharp [FluentRule.cs]
RuleFor(x => x.DisplayName)
    .Min(3);
```

:::

## Max

Requires the value to be no greater than the provided number. For strings, collections, and files, the number is interpreted as length, count, or kilobytes.

::: code-group

```csharp [Attribute.cs]
[Max(double value)]

[Max(255)]
```

```csharp [FluentRule.cs]
RuleFor(x => x.DisplayName)
    .Max(255);
```

:::

## Between

Requires the value to be between the inclusive minimum and maximum.

::: code-group

```csharp [Attribute.cs]
[Between(double min, double max)]

[Between(1, 365)]
```

```csharp [FluentRule.cs]
RuleFor(x => x.DurationDays)
    .Between(1, 365);
```

:::

## Size

Requires the value to match the exact numeric value, string length, collection count, or file size.

::: code-group

```csharp [Attribute.cs]
[Size(double value)]

[Size(6)]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Code)
    .Size(6);
```

:::

## Decimal

Requires the numeric value to have the configured number of decimal places.

::: code-group

```csharp [Attribute.cs]
[Decimal(int places)]

[Decimal(2)]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Price)
    .Decimal(2);
```

:::

## Digits

Requires the value to contain exactly the provided number of digits.

::: code-group

```csharp [Attribute.cs]
[Digits(int digits)]

[Digits(8)]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Pin)
    .Digits(8);
```

:::

## DigitsBetween

Requires the digit count to be between the inclusive minimum and maximum.

::: code-group

```csharp [Attribute.cs]
[DigitsBetween(int min, int max)]

[DigitsBetween(8, 12)]
```

```csharp [FluentRule.cs]
RuleFor(x => x.AccountNumber)
    .DigitsBetween(8, 12);
```

:::

## MinDigits

Requires the value to contain at least the provided number of digits.

::: code-group

```csharp [Attribute.cs]
[MinDigits(int min)]

[MinDigits(8)]
```

```csharp [FluentRule.cs]
RuleFor(x => x.AccountNumber)
    .MinDigits(8);
```

:::

## MaxDigits

Allows the value to contain at most the provided number of digits.

::: code-group

```csharp [Attribute.cs]
[MaxDigits(int max)]

[MaxDigits(12)]
```

```csharp [FluentRule.cs]
RuleFor(x => x.AccountNumber)
    .MaxDigits(12);
```

:::

## MultipleOf

Requires the numeric value to be a multiple of the provided number.

::: code-group

```csharp [Attribute.cs]
[MultipleOf(double value)]

[MultipleOf(5)]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Quantity)
    .MultipleOf(5);
```

:::

## GreaterThan

Requires the value to be greater than the provided number.

::: code-group

```csharp [Attribute.cs]
[GreaterThan(double value)]

[GreaterThan(0)]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Price)
    .GreaterThan(0);
```

:::

## GreaterThanOrEqual

Requires the value to be greater than or equal to the provided number.

::: code-group

```csharp [Attribute.cs]
[GreaterThanOrEqual(double value)]

[GreaterThanOrEqual(18)]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Age)
    .GreaterThanOrEqual(18);
```

:::

## LessThan

Requires the value to be less than the provided number.

::: code-group

```csharp [Attribute.cs]
[LessThan(double value)]

[LessThan(100)]
```

```csharp [FluentRule.cs]
RuleFor(x => x.DiscountPercentage)
    .LessThan(100);
```

:::

## LessThanOrEqual

Requires the value to be less than or equal to the provided number.

::: code-group

```csharp [Attribute.cs]
[LessThanOrEqual(double value)]

[LessThanOrEqual(100)]
```

```csharp [FluentRule.cs]
RuleFor(x => x.DiscountPercentage)
    .LessThanOrEqual(100);
```

:::
