---
params:
    - name: navigation
      description: The navigation to load eagerly. It has to be a navigation the model already holds, so configure the relationship before marking it auto-included.
      type: 'Expression<Func<TEntity, object?>>'

returns: The `ModelBuilder` instance with the navigation set to load eagerly.
---

# ApplyAutoInclude

Configures a navigation to be loaded whenever the entity itself is queried, without an explicit `Include`. The include lives in the model rather than the query, so it applies to every read of that entity, including reads written in another project that only references it. A query whose results are not the entity, such as a `CountAsync` or a projection, emits no join for it.

## Usage

::: code-group

```csharp [AppDbContext.cs]
using AlmightyShogun.EntityFrameworkCore.ModelBuilding;

modelBuilder.ApplyAutoInclude<Order>(order => order.Country);
```

```csharp [OptingOut.cs]
using Microsoft.EntityFrameworkCore;

List<Order> orders = await database.Orders
    .IgnoreAutoIncludes()
    .ToListAsync(cancellationToken);
```

:::

::: warning
Every query returning the entity loads the navigation, so an expensive navigation, or a chain of them, makes reads that never asked for the data pay for it. Reach for it on something small and almost always needed, such as a lookup or a translation row, and call `IgnoreAutoIncludes()` on a query that has to opt out.
:::

<FrontmatterDocs/>

## Type signature

```csharp
public ModelBuilder ApplyAutoInclude<TEntity>(
    Expression<Func<TEntity, object?>> navigation
) where TEntity : class;
```
