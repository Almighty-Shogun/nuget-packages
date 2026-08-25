# ComparisonTarget

Defines how validation rules resolve a comparison target. Use `Value` when the target is a literal comparison value. Use `Field` when the target is the name of another request property.

## Usage

```csharp
using AlmightyShogun.AspNet.RequestValidation;

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
