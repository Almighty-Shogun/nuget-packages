# Fluent Validation

Fluent validation is used by deriving a request type from `ValidatableRequest<TRequest>` and implementing `Rules`. It is useful when validation needs expression-based field references, grouped alternatives, custom DI-backed rules, or a rule chain that is easier to read than stacking attributes on a property.

Attribute rules and fluent rules can be used on the same request type. The package merges rules for the same field, removes duplicate rule identities, runs required-style rules before normal rules, and returns the first failing rule per field.

## Usage

```csharp
using AlmightyShogun.AspNet.Validation;

public sealed class CreateAccountRequest : ValidatableRequest<CreateAccountRequest>
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string PasswordConfirmation { get; init; } = string.Empty;
    public string[] Roles { get; init; } = [];

    protected override void Rules()
    {
        RuleFor(x => x.Email)
            .Required()
            .Email()
            .Max(120);

        RuleFor(x => x.Password)
            .Required()
            .PasswordSecure()
            .Confirmed(x => x.PasswordConfirmation);

        RuleFor(x => x.Roles)
            .Array()
            .Distinct();
    }
}
```

Required text fields in the example use non-nullable properties with empty defaults so the C# type shape matches the validation intent. Use nullable properties when the request must preserve a missing value for validation, such as conditionally-required fields, uploaded files, or nullable value types.

`RuleFor` accepts a direct property access expression. Nested calculations, method calls, and arbitrary expressions are rejected because the package needs a stable request field name for the validation response.

## ValidatableRequest

`ValidatableRequest<TRequest>` is the base class for request-owned fluent rules. Application code derives from it and overrides `Rules`; the package calls that method once per request type and caches the resulting rule set.

```csharp
using AlmightyShogun.AspNet.Validation;

public sealed class UpdateProfileRequest : ValidatableRequest<UpdateProfileRequest>
{
    public string DisplayName { get; init; } = string.Empty;

    protected override void Rules()
    {
        RuleFor(x => x.DisplayName)
            .Required()
            .String()
            .Max(80);
    }
}
```

Use [Validation Rules](./validation-rules/presence) for the complete list of rule families and side-by-side attribute/fluent examples.

## Type signature

```csharp
public abstract class ValidatableRequest<TRequest>
    where TRequest : ValidatableRequest<TRequest>
{
    protected abstract void Rules();

    protected RuleBuilder<TRequest, TProperty> RuleFor<TProperty>(
        Expression<Func<TRequest, TProperty>> expression
    );
}
```
