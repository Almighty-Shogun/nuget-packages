---
params:
    - name: index
      description: The property to index, or an anonymous object of properties for a composite index. Column order in a composite index is the order given, and only a leading subset of it can be used by a query.
      type: 'Expression<Func<TEntity, object?>>'

returns: The `ModelBuilder` instance with the index configured.
---

# ApplyIndex

Adds a non-unique index over one property or a set of them. Use [`ApplyUniqueIndex`](./apply-unique-index) when the index also has to enforce uniqueness.

## Usage

::: code-group

```csharp [Single.cs]
using AlmightyShogun.EntityFrameworkCore.ModelBuilding;

modelBuilder.ApplyIndex<Order>(order => order.PlacedAt);
```

```csharp [Composite.cs]
using AlmightyShogun.EntityFrameworkCore.ModelBuilding;

modelBuilder.ApplyIndex<Order>(order => new
{
    order.AccountId,
    order.PlacedAt
});
```

:::

::: tip
Call `HasIndex` directly for an index that needs a database name, an include list, or a filter. Naming every option here would only restate the fluent API a call at a time.
:::

<FrontmatterDocs/>

## Type signature

```csharp
public ModelBuilder ApplyIndex<TEntity>(
    Expression<Func<TEntity, object?>> index
) where TEntity : class;
```
