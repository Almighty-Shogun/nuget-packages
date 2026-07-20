# ComparisonTarget

Defines how validation rules resolve a comparison target. Use `Value` when the target is a literal comparison value. Use `Field` when the target is the name of another request property.

The date attributes currently use this enum for string-based comparison targets. Custom rules can also use it when they expose APIs that need to distinguish literal values from field names.

## Usage

```csharp
using AlmightyShogun.AspNet.Validation;

public sealed class ScheduleEventRequest
{
    [Before("EndsAt", ComparisonTarget.Field)]
    public DateTimeOffset? StartsAt { get; init; }

    public DateTimeOffset? EndsAt { get; init; }
}
```

## Type signature

```csharp
public enum ComparisonTarget
{
    Value,
    Field
}
```
