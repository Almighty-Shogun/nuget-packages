# Conditional Presence Rules

Conditional presence rules apply required, present, missing, or prohibited behavior only when other request fields match a condition. They are useful when one field changes the meaning of another field, such as payment method details, scheduled publishing, or mutually exclusive options.

Attribute parameters use request property names. Fluent rules use property expressions, which gives compile-time refactoring support. Conditionally-required fields usually stay nullable in request DTOs because the value is allowed to be missing until the configured condition is true.

## RequiredIf

Requires the field when another field equals one of the provided values.

::: code-group

```csharp [Attribute.cs]
[RequiredIf(string field, params object?[] values)]

[RequiredIf("Status", "scheduled")]
```

```csharp [FluentRule.cs]
RuleFor(x => x.PublishAt)
    .RequiredIf(x => x.Status, "scheduled");
```

:::

## RequiredUnless

Requires the field unless another field equals one of the provided values.

::: code-group

```csharp [Attribute.cs]
[RequiredUnless(string field, params object?[] values)]

[RequiredUnless("Status", "draft")]
```

```csharp [FluentRule.cs]
RuleFor(x => x.ApprovalReason)
    .RequiredUnless(x => x.Status, "draft");
```

:::

## RequiredIfAccepted

Requires the field when another field contains an accepted value such as `true`, `yes`, `on`, or `1`.

::: code-group

```csharp [Attribute.cs]
[RequiredIfAccepted(string field)]

[RequiredIfAccepted("TermsAccepted")]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Signature)
    .RequiredIfAccepted(x => x.TermsAccepted);
```

:::

## RequiredIfDeclined

Requires the field when another field contains a declined value such as `false`, `no`, `off`, or `0`.

::: code-group

```csharp [Attribute.cs]
[RequiredIfDeclined(string field)]

[RequiredIfDeclined("MarketingAccepted")]
```

```csharp [FluentRule.cs]
RuleFor(x => x.DeclineReason)
    .RequiredIfDeclined(x => x.MarketingAccepted);
```

:::

## RequiredWith

Requires the field when any listed field is present.

::: code-group

```csharp [Attribute.cs]
[RequiredWith(params string[] fields)]

[RequiredWith("Email", "PhoneNumber")]
```

```csharp [FluentRule.cs]
RuleFor(x => x.ContactName)
    .RequiredWith(x => x.Email, x => x.PhoneNumber);
```

:::

## RequiredWithAll

Requires the field when all listed fields are present.

::: code-group

```csharp [Attribute.cs]
[RequiredWithAll(params string[] fields)]

[RequiredWithAll("StartsAt", "EndsAt")]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Timezone)
    .RequiredWithAll(x => x.StartsAt, x => x.EndsAt);
```

:::

## RequiredWithout

Requires the field when any listed field is missing.

::: code-group

```csharp [Attribute.cs]
[RequiredWithout(params string[] fields)]

[RequiredWithout("Email", "PhoneNumber")]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Email)
    .RequiredWithout(x => x.PhoneNumber);
```

:::

## RequiredWithoutAll

Requires the field when all listed fields are missing.

::: code-group

```csharp [Attribute.cs]
[RequiredWithoutAll(params string[] fields)]

[RequiredWithoutAll("Email", "PhoneNumber")]
```

```csharp [FluentRule.cs]
RuleFor(x => x.ContactName)
    .RequiredWithoutAll(x => x.Email, x => x.PhoneNumber);
```

:::

## PresentIf

Requires the field to be present when another field equals one of the provided values. The field may still be empty unless another rule rejects empty values.

::: code-group

```csharp [Attribute.cs]
[PresentIf(string field, params object?[] values)]

[PresentIf("Mode", "manual")]
```

```csharp [FluentRule.cs]
RuleFor(x => x.ManualNote)
    .PresentIf(x => x.Mode, "manual");
```

:::

## PresentUnless

Requires the field to be present unless another field equals one of the provided values.

::: code-group

```csharp [Attribute.cs]
[PresentUnless(string field, params object?[] values)]

[PresentUnless("Mode", "automatic")]
```

```csharp [FluentRule.cs]
RuleFor(x => x.ExternalReference)
    .PresentUnless(x => x.Mode, "automatic");
```

:::

## PresentWith

Requires the field to be present when any listed field is present.

::: code-group

```csharp [Attribute.cs]
[PresentWith(params string[] fields)]

[PresentWith("Street", "City")]
```

```csharp [FluentRule.cs]
RuleFor(x => x.AddressLine2)
    .PresentWith(x => x.Street, x => x.City);
```

:::

## PresentWithAll

Requires the field to be present when all listed fields are present.

::: code-group

```csharp [Attribute.cs]
[PresentWithAll(params string[] fields)]

[PresentWithAll("Street", "PostalCode")]
```

```csharp [FluentRule.cs]
RuleFor(x => x.AddressNote)
    .PresentWithAll(x => x.Street, x => x.PostalCode);
```

:::

## MissingIf

Requires the field to be missing when another field equals one of the provided values.

::: code-group

```csharp [Attribute.cs]
[MissingIf(string field, params object?[] values)]

[MissingIf("AccountType", "guest")]
```

```csharp [FluentRule.cs]
RuleFor(x => x.BusinessNumber)
    .MissingIf(x => x.AccountType, "guest");
```

:::

## MissingUnless

Requires the field to be missing unless another field equals one of the provided values.

::: code-group

```csharp [Attribute.cs]
[MissingUnless(string field, params object?[] values)]

[MissingUnless("AccountType", "business")]
```

```csharp [FluentRule.cs]
RuleFor(x => x.BusinessNumber)
    .MissingUnless(x => x.AccountType, "business");
```

:::

## MissingWith

Requires the field to be missing when any listed field is present.

::: code-group

```csharp [Attribute.cs]
[MissingWith(params string[] fields)]

[MissingWith("ArchiveReason")]
```

```csharp [FluentRule.cs]
RuleFor(x => x.DeleteReason)
    .MissingWith(x => x.ArchiveReason);
```

:::

## MissingWithAll

Requires the field to be missing when all listed fields are present.

::: code-group

```csharp [Attribute.cs]
[MissingWithAll(params string[] fields)]

[MissingWithAll("StartsAt", "EndsAt")]
```

```csharp [FluentRule.cs]
RuleFor(x => x.PublishImmediately)
    .MissingWithAll(x => x.StartsAt, x => x.EndsAt);
```

:::

## ProhibitedIf

Prohibits the field when another field equals one of the provided values.

::: code-group

```csharp [Attribute.cs]
[ProhibitedIf(string field, params object?[] values)]

[ProhibitedIf("AccountType", "guest")]
```

```csharp [FluentRule.cs]
RuleFor(x => x.BusinessNumber)
    .ProhibitedIf(x => x.AccountType, "guest");
```

:::

## ProhibitedUnless

Prohibits the field unless another field equals one of the provided values.

::: code-group

```csharp [Attribute.cs]
[ProhibitedUnless(string field, params object?[] values)]

[ProhibitedUnless("AccountType", "business")]
```

```csharp [FluentRule.cs]
RuleFor(x => x.BusinessNumber)
    .ProhibitedUnless(x => x.AccountType, "business");
```

:::

## ProhibitedIfAccepted

Prohibits the field when another field contains an accepted value.

::: code-group

```csharp [Attribute.cs]
[ProhibitedIfAccepted(string field)]

[ProhibitedIfAccepted("UseDefaultBilling")]
```

```csharp [FluentRule.cs]
RuleFor(x => x.BillingAddress)
    .ProhibitedIfAccepted(x => x.UseDefaultBilling);
```

:::

## ProhibitedIfDeclined

Prohibits the field when another field contains a declined value.

::: code-group

```csharp [Attribute.cs]
[ProhibitedIfDeclined(string field)]

[ProhibitedIfDeclined("CustomBillingEnabled")]
```

```csharp [FluentRule.cs]
RuleFor(x => x.CustomBillingAddress)
    .ProhibitedIfDeclined(x => x.CustomBillingEnabled);
```

:::

## Prohibits

Makes this field prohibit the listed fields from being present with a value. Use it for mutually exclusive request options.

::: code-group

```csharp [Attribute.cs]
[Prohibits(params string[] fields)]

[Prohibits("ScheduledAt", "PublishImmediately")]
```

```csharp [FluentRule.cs]
RuleFor(x => x.PublishImmediately)
    .Prohibits(x => x.ScheduledAt);
```

:::
