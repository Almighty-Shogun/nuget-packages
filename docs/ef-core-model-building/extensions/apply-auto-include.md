---
params:
    - name: navigation
      description: The navigation property to load automatically.
      type: 'Expression<Func<TEntity, object?>>'

returns: The same `ModelBuilder` instance.
---

# ApplyAutoInclude

Configures a navigation to be loaded whenever the entity itself is queried, without an explicit `Include`. The include lives in the model rather than the query, so it applies to every read of that entity, including reads written in another project that only references it.

::: warning
This applies to **every** query that returns the entity, including ones written elsewhere against a model this package configured. A navigation that is expensive, or a chain of them, makes reads that never asked for the data pay for it.

Use it for something small and almost always needed, such as a lookup or a translation row. Call `IgnoreAutoIncludes()` on a query to opt out once.
:::

::: tip
A query whose results are not the entity is unaffected. A `CountAsync`, or a projection such as `Select(order => order.Id)`, emits no join for an auto-included navigation, so it costs nothing there.
:::

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

<FrontmatterDocs/>

## Type signature

```csharp
public ModelBuilder ApplyAutoInclude<TEntity>(
    Expression<Func<TEntity, object?>> navigation
) where TEntity : class;
```
