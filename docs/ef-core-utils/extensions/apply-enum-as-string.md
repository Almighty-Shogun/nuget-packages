---
params:
    - name: property
      description: The enum property to configure.
      type: 'Expression<Func<TEntity, TProperty>>'
    - name: maxLength
      description: The maximum stored length. Raise it when a member name is longer than the default.
      type: int
      default: '32'

returns: The same `ModelBuilder` instance.
---

# ApplyEnumAsString

Stores an enum property as its member name rather than its underlying number.

Entity Framework Core stores an enum as an integer by default, so inserting a member in the middle re-points every existing row at a different member with no error and no migration change. The cost of storing the name is a text column and a text comparison.

## Usage

::: code-group

```csharp [AppDbContext.cs]
using AlmightyShogun.EntityFrameworkCore.Utils;

modelBuilder.ApplyEnumAsString<Order, Status>(order => order.Status);
```

```csharp [LongerNames.cs]
using AlmightyShogun.EntityFrameworkCore.Utils;

modelBuilder.ApplyEnumAsString<Order, FulfilmentStage>(
    order => order.Stage,
    maxLength: 64
);
```

```csharp [Order.cs]
public sealed class Order
{
    public int Id { get; set; }
    public Status Status { get; set; }
    public FulfilmentStage Stage { get; set; }
}

public enum Status
{
    Pending,
    Shipped,
    Cancelled
}

public enum FulfilmentStage
{
    AwaitingPaymentConfirmation,
    ReadyForWarehousePicking,
    HandedToCarrier
}
```

:::

::: warning
Renaming a member changes the stored value, so it needs an `UPDATE` in the migration that renames it.
:::

<FrontmatterDocs/>

## Type signature

```csharp
public ModelBuilder ApplyEnumAsString<TEntity, TProperty>(
    Expression<Func<TEntity, TProperty>> property,
    int maxLength = 32
) where TEntity : class where TProperty : struct, Enum;
```
