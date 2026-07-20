# Custom Rules

Custom rules let validation use application services while still producing the same localized validation error response as built-in rules. Use them for checks that need database access, tenant state, an external service, or application logic that does not belong inside an attribute constructor.

Custom rules are DI-resolved classes that implement `ICustomValidationRule<TRequest, TProperty>`. They return [`ValidationRuleResult.Success`](./records/validation-rule-result) when the value is valid, or [`ValidationRuleResult.Failure`](./records/validation-rule-result) with a message key and optional message parameters when validation fails.

## CustomRule

Adds a custom validation rule resolved from dependency injection. Use the attribute form when the rule can sit directly on a DTO property. Use the fluent form when the rule belongs in a `ValidatableRequest<TRequest>` chain with other expression-based rules.

::: code-group

```csharp [Attribute.cs]
[CustomRule<UniqueEmailRule>]
public string Email { get; init; } = string.Empty;
```

```csharp [FluentRule.cs]
RuleFor(x => x.Email)
    .CustomRule<UniqueEmailRule>();
```

:::

## Create a custom rule type

Use `.CustomRule<TRule>()` from a `ValidatableRequest<TRequest>` when the rule only belongs to that request or when fluent validation reads better than attributes.

::: code-group

```csharp [CreateAccountRequest.cs]
using AlmightyShogun.AspNet.Validation;

public sealed class CreateAccountRequest : ValidatableRequest<CreateAccountRequest>
{
    public string Email { get; init; } = string.Empty;

    protected override void Rules()
    {
        RuleFor(request => request.Email)
            .Required()
            .Email()
            .CustomRule<UniqueEmailRule>();
    }
}
```

```csharp [UserRepository.cs]
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

public sealed class UserRepository
{
    private static readonly HashSet<string> Emails = new(StringComparer.OrdinalIgnoreCase)
    {
        "admin@example.com"
    };

    public Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
        => Task.FromResult(Emails.Contains(email));
}
```

```csharp [UniqueEmailRule.cs]
using System.Threading;
using System.Threading.Tasks;
using AlmightyShogun.AspNet.Validation;

public sealed class UniqueEmailRule(UserRepository userRepository)
    : ICustomValidationRule<CreateAccountRequest, string>
{
    public async Task<ValidationRuleResult> ValidateAsync(
        CreateAccountRequest request,
        string? value,
        CancellationToken cancellationToken = default
    )
    {
        if (string.IsNullOrWhiteSpace(value))
            return ValidationRuleResult.Success();

        bool exists = await userRepository.EmailExistsAsync(value, cancellationToken);

        return exists
            ? ValidationRuleResult.Failure("validation.unique")
            : ValidationRuleResult.Success();
    }
}
```

:::

The failure key is resolved through the message files described on the [Localization](./localization) page. When the key cannot be resolved, the response contains the key itself, which keeps the API response stable during missing translation work.

## Create a domain-specific attribute

Use `CustomRuleAttribute<TRule>` when the same DI-backed rule should be attached directly to a request property. This keeps the request DTO compact when the validation rule is reusable and does not need fluent expression setup.

```csharp
using AlmightyShogun.AspNet.Validation;

public sealed class CreateAccountRequest
{
    [Required]
    [Email]
    [CustomRule<UniqueEmailRule>]
    public string Email { get; init; } = string.Empty;
}
```

Derive from `CustomRuleAttribute` when an application wants a domain-specific attribute name instead of writing `[CustomRule<TRule>]` everywhere.

::: code-group

```csharp [UniqueEmailAttribute.cs]
using System;
using AlmightyShogun.AspNet.Validation;

public sealed class UniqueEmailAttribute : CustomRuleAttribute
{
    protected override Type CreateCustomRule() => CustomRule<UniqueEmailRule>();
}
```

```csharp [RegisterAccountRequest.cs]
using AlmightyShogun.AspNet.Validation;

public sealed class RegisterAccountRequest
{
    [Required]
    [Email]
    [UniqueEmail]
    public string Email { get; init; } = string.Empty;
}
```

:::

## Type signature

```csharp
public interface ICustomValidationRule<in TRequest, in TProperty>
    where TRequest : class
{
    Task<ValidationRuleResult> ValidateAsync(
        TRequest request,
        TProperty? value,
        CancellationToken cancellationToken = default
    );
}

public abstract class CustomRuleAttribute : ValidationRuleAttribute
{
    protected CustomRuleAttribute();

    protected abstract Type CreateCustomRule();

    protected static Type CustomRule<TRule>()
        where TRule : class;
}

public sealed class CustomRuleAttribute<TRule> : CustomRuleAttribute
    where TRule : class;
```
