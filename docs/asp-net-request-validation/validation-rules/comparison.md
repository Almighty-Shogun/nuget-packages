# Comparison Rules

Comparison rules compare a field against another field, a conditional value, an allowed set, object keys, or the other values inside the same collection. They are useful for confirmations, consent fields, repeated values, and request fields that must agree with each other.

## Accepted

Requires the field to contain an accepted value such as `true`, `yes`, `on`, or `1`.

::: code-group

```csharp [Attribute.cs]
[Accepted]
```

```csharp [FluentRule.cs]
RuleFor(x => x.TermsAccepted)
    .Accepted();
```

:::

## AcceptedIf

Requires the field to contain an accepted value when another field equals one of the provided values. An absent field fails once the condition matches, the same way an unticked checkbox fails [`[Accepted]`](#accepted).

::: code-group

```csharp [Attribute.cs]
[AcceptedIf(string field, params object?[] values)]

[AcceptedIf("NewsletterEnabled", true)]
```

```csharp [FluentRule.cs]
RuleFor(x => x.MarketingConsent)
    .AcceptedIf(x => x.NewsletterEnabled, true);
```

:::

## Declined

Requires the field to contain a declined value such as `false`, `no`, `off`, or `0`.

::: code-group

```csharp [Attribute.cs]
[Declined]
```

```csharp [FluentRule.cs]
RuleFor(x => x.AgeRestricted)
    .Declined();
```

:::

## DeclinedIf

Requires the field to contain a declined value when another field equals one of the provided values. An absent field fails once the condition matches, the same way it does for [`[Declined]`](#declined).

::: code-group

```csharp [Attribute.cs]
[DeclinedIf(string field, params object?[] values)]

[DeclinedIf("AgeRestricted", true)]
```

```csharp [FluentRule.cs]
RuleFor(x => x.ParentalApprovalDeclined)
    .DeclinedIf(x => x.AgeRestricted, true);
```

:::

## SameAs

Requires the field value to match another request field.

::: code-group

```csharp [Attribute.cs]
[SameAs(string field)]

[SameAs("Email")]
```

```csharp [FluentRule.cs]
RuleFor(x => x.EmailConfirmation)
    .SameAs(x => x.Email);
```

:::

## Different

Requires the field value to be different from another request field.

::: code-group

```csharp [Attribute.cs]
[Different(string field)]

[Different("CurrentPassword")]
```

```csharp [FluentRule.cs]
RuleFor(x => x.NewPassword)
    .Different(x => x.CurrentPassword);
```

:::

## Confirmed

Requires the field to match a confirmation field. Without an explicit target, the validator uses the property name with `Confirmation` appended.

::: code-group

```csharp [Attribute.cs]
[Confirmed(string? field = null)]

[Confirmed]
[Confirmed("RepeatPassword")]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Password)
    .Confirmed(x => x.PasswordConfirmation);
```

:::

## In

Requires the field value to be inside a set of allowed values.

::: code-group

```csharp [Attribute.cs]
[In(params object?[] values)]

[In("admin", "editor")]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Role)
    .In("admin", "editor");
```

:::

## NotIn

Requires the field value to be outside a set of forbidden values.

::: code-group

```csharp [Attribute.cs]
[NotIn(params object?[] values)]

[NotIn("admin", "system")]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Username)
    .NotIn("admin", "system");
```

:::

## InArray

Requires the field value to exist in another array-like request field.

::: code-group

```csharp [Attribute.cs]
[InArray(string field)]

[InArray("AllowedRoleIds")]
```

```csharp [FluentRule.cs]
RuleFor(x => x.RoleId)
    .InArray(x => x.AllowedRoleIds);
```

:::

## InArrayKeys

Requires an object or dictionary-like field to contain at least one of the provided keys.

::: code-group

```csharp [Attribute.cs]
[InArrayKeys(params string[] keys)]

[InArrayKeys("theme", "locale")]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Settings)
    .InArrayKeys("theme", "locale");
```

:::

## RequiredArrayKeys

Requires an object or dictionary-like field to contain all provided keys.

::: code-group

```csharp [Attribute.cs]
[RequiredArrayKeys(params string[] keys)]

[RequiredArrayKeys("street", "city", "postalCode")]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Address)
    .RequiredArrayKeys("street", "city", "postalCode");
```

:::

## Distinct

Requires all values in an array-like field to be unique.

::: code-group

```csharp [Attribute.cs]
[Distinct]
```

```csharp [FluentRule.cs]
RuleFor(x => x.RoleIds)
    .Distinct();
```

:::

## Regex

Requires the value to match the regular expression pattern.

The optional `description` is passed to the message template as `{0}`, so a failure can state the expected shape instead of only saying the value is invalid.

Each match runs under a timeout, one second by default, after which the match is abandoned and the rule fails. This bounds catastrophic backtracking on attacker-supplied input, which would otherwise pin a CPU core with no way to cancel it. Raise `matchTimeoutSeconds` only for a pattern that legitimately needs longer.

::: code-group

```csharp [Attribute.cs]
[Regex(string pattern, RegexOptions options = RegexOptions.None, string? description = null, double matchTimeoutSeconds = 1)]

[Regex("^[a-z0-9-]+$", RegexOptions.IgnoreCase, "lowercase letters, digits, and hyphens")]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Slug)
    .Regex("^[a-z0-9-]+$", RegexOptions.IgnoreCase, "lowercase letters, digits, and hyphens");
```

:::

## NotRegex

Requires the value to not match the regular expression pattern.

Takes the same optional `description` and match timeout as [Regex](#regex).

::: code-group

```csharp [Attribute.cs]
[NotRegex(string pattern, RegexOptions options = RegexOptions.None, string? description = null, double matchTimeoutSeconds = 1)]

[NotRegex("^admin-", RegexOptions.None, "must not start with admin-")]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Username)
    .NotRegex("^admin-", description: "must not start with admin-");
```

:::
