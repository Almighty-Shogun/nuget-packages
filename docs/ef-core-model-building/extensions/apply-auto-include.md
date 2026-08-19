---
params:
    - name: navigation
      description: The navigation property to load automatically.
      type: 'Expression<Func<TEntity, object?>>'

returns: The same `ModelBuilder` instance.
---

# ApplyAutoInclude

Configures a navigation to be loaded on every query for the entity, without an explicit `Include`. The include lives in the model rather than the query, so it applies to every read of that entity, including reads written in another project that only references it.

::: warning
This applies to **every** query for that entity, including ones that only need a count or a single column. A navigation that is expensive, or a chain of them, makes queries that never asked for the data pay for it.

Use it for something small and almost always needed, such as a lookup or a translation row. Call `IgnoreAutoIncludes()` on a query to opt out once.
:::

## Usage

::: code-group

```csharp [AppDbContext.cs]
using AlmightyShogun.EntityFrameworkCore.ModelBuilding;

modelBuilder.ApplyAutoInclude<Order>(order => order.Country);
```

```csharp [OptingOut.cs]
using Microsoft.EntityFrameworkCore;

int count = await database.Orders
    .IgnoreAutoIncludes()
    .CountAsync(cancellationToken);
```

:::

<FrontmatterDocs/>

## Type signature

```csharp
public ModelBuilder ApplyAutoInclude<TEntity>(
    Expression<Func<TEntity, object?>> navigation
) where TEntity : class;
```
