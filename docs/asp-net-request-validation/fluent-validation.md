# Fluent Validation

Fluent validation is used by writing a `Validator<TRequest>` for a request type and implementing `Rules`. It is useful when validation needs expression-based field references, grouped alternatives, custom DI-backed rules, or a rule chain that is easier to read than stacking attributes on a property. A validator needs no registration of its own, since [`AddAspNetValidation`](./extensions/add-asp-net-validation) scans for validators and pairs each one with the request it names.

## Usage

::: code-group

```csharp [CreateAccountRequest.cs]
public sealed class CreateAccountRequest
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string PasswordConfirmation { get; init; } = string.Empty;
    public string[] Roles { get; init; } = [];
}
```

```csharp [CreateAccountRequestValidator.cs]
using AlmightyShogun.AspNet.RequestValidation;

public sealed class CreateAccountRequestValidator
    : Validator<CreateAccountRequest>
{
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

:::

## Validator

`Validator<TRequest>` is the base class for fluent rules. Application code derives from it and overrides `Rules`; the package calls that method once per request type and caches the resulting rule set.

::: code-group

```csharp [UpdateProfileRequest.cs]
public sealed class UpdateProfileRequest
{
    public string DisplayName { get; init; } = string.Empty;
}
```

```csharp [UpdateProfileRequestValidator.cs]
using AlmightyShogun.AspNet.RequestValidation;

public sealed class UpdateProfileRequestValidator
    : Validator<UpdateProfileRequest>
{
    protected override void Rules()
    {
        RuleFor(x => x.DisplayName)
            .Required()
            .String()
            .Max(80);
    }
}
```

:::

Use [Validation Rules](./validation-rules/presence) for the complete list of rule families and side-by-side attribute/fluent examples.

## Rules and attributes are combined

A request may carry attributes and have a validator. Both sets are merged per field, with the attribute rules first, and a constraint declared in both places is checked once rather than reported twice.

::: code-group

```csharp [NoteRequest.cs]
using AlmightyShogun.AspNet.RequestValidation;

public sealed class NoteRequest
{
    [Required]
    [Email]
    public string Email { get; init; } = string.Empty;

    public string? Note { get; init; }
}
```

```csharp [NoteRequestValidator.cs]
using AlmightyShogun.AspNet.RequestValidation;

public sealed class NoteRequestValidator : Validator<NoteRequest>
{
    protected override void Rules() => RuleFor(x => x.Note).Max(500);
}
```

:::

## Rules cannot depend on a request instance

`Rules` runs once for the request type and the result is cached for the life of the process, so it must not branch on the values of any one request. A validator holds no request, which makes that a compile error rather than a bug that only shows on the second request:

```csharp
protected override void Rules()
{
    // Does not compile: there is no request instance to read.
    if (IsCompany) RuleFor(x => x.CompanyName).Required();

    // Correct: the requirement itself is conditional.
    RuleFor(x => x.CompanyName).RequiredIf(x => x.IsCompany, true);
}
```

For the same reason a validator takes no dependencies and needs a public parameterless constructor. It runs outside any request scope, so a service captured there would outlive the scope it came from. A rule that genuinely needs services belongs in a [custom rule](./custom-rules), which is resolved per request.

## Only direct property reads

`RuleFor` accepts a property read straight off the request. A nested read is refused when the rule is built, rather than being accepted and then naming the field after the leaf property and throwing whenever an intermediate value is null:

```csharp
RuleFor(x => x.Email);        // fine
RuleFor(x => x.User.Email);   // throws ArgumentOutOfRangeException
```

## One validator per request

Two validators naming the same request type is refused at startup, so neither silently wins. A validator for a base request does not cover a type derived from it, since its rules are expressions over the base.

## Type signature

```csharp
public abstract class Validator<TRequest>
    where TRequest : class
{
    protected abstract void Rules();

    protected RuleBuilder<TRequest, TProperty> RuleFor<TProperty>(
        Expression<Func<TRequest, TProperty>> expression
    );
}
```
