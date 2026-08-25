# Password Rules

Password rules validate common password composition requirements. They only inspect the submitted value; they do not hash passwords, compare stored passwords, or enforce account-specific password history.

Use `PasswordSecure` for the normal strict requirement, or combine smaller password rules when the application controls the exact password policy.

## Password

Adds several password requirements at once. By default it requires letters, mixed casing, numbers, and symbols; pass `false` for a requirement to skip that part.

::: code-group

```csharp [Attribute.cs]
[PasswordLetters]
[PasswordMixed]
[PasswordNumbers]
[PasswordSymbols]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Password)
    .Password();
```

:::

## PasswordLetters

Requires the password to contain at least one letter.

::: code-group

```csharp [Attribute.cs]
[PasswordLetters]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Password)
    .PasswordLetters();
```

:::

## PasswordMixed

Requires the password to contain at least one lowercase and one uppercase letter.

::: code-group

```csharp [Attribute.cs]
[PasswordMixed]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Password)
    .PasswordMixed();
```

:::

## PasswordNumbers

Requires the password to contain at least one number.

::: code-group

```csharp [Attribute.cs]
[PasswordNumbers]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Password)
    .PasswordNumbers();
```

:::

## PasswordSymbols

Requires the password to contain at least one punctuation or symbol character.

::: code-group

```csharp [Attribute.cs]
[PasswordSymbols]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Password)
    .PasswordSymbols();
```

:::

## PasswordSecure

Requires the password to contain mixed-case letters, at least one number, and at least one symbol.

::: code-group

```csharp [Attribute.cs]
[PasswordSecure]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Password)
    .PasswordSecure();
```

:::
