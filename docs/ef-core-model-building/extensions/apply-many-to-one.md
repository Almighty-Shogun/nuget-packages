---
params:
    - name: navigation
      description: Reference navigation on the dependent entity pointing to the principal.
      type: 'Expression<Func<TDependent, TEntity?>>'
    - name: foreignKey
      description: Foreign key property on the dependent entity.
      type: 'Expression<Func<TDependent, object?>>'
    - name: principalKey
      description: Principal key property. When omitted, the principal's primary key is used.
      type: 'Expression<Func<TEntity, object?>>?'
      default: 'null'
    - name: isRequired
      description: Whether the relationship is required.
      type: bool
      default: 'false'
    - name: deleteBehavior
      description: What happens to the dependents when the principal is deleted.
      type: DeleteBehavior
      default: DeleteBehavior.ClientSetNull
    - name: inverseNavigation
      description: Collection navigation on the principal containing the dependents.
      type: 'Expression<Func<TEntity, IEnumerable<TDependent>?>>?'
      default: 'null'

returns: The same `ModelBuilder` instance.
---

# ApplyManyToOne

The same relationship as [`ApplyOneToMany`](./apply-one-to-many), configured from the dependent's side.

Use it when the dependent holds the navigation and the principal does not, which is the usual shape for a lookup table: an `Order` points at a `Country`, and `Country` holds no collection of orders. Both helpers produce the same schema, so the choice is only about which side the navigation is written on.

## Usage

::: code-group

```csharp [AppDbContext.cs]
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.EntityFrameworkCore.ModelBuilding;

modelBuilder.ApplyManyToOne<Country, Order>(
    order => order.Country,
    order => order.CountryId
);
```

```csharp [Entities.cs]
public sealed class Country
{
    public int Id { get; set; }
    public required string Name { get; set; }
}

public sealed class Order
{
    public int Id { get; set; }
    public int? CountryId { get; set; }
    public Country? Country { get; set; }
}
```

:::

<FrontmatterDocs/>

## Type signature

```csharp
public ModelBuilder ApplyManyToOne<TEntity, TDependent>(
    Expression<Func<TDependent, TEntity?>> navigation,
    Expression<Func<TDependent, object?>> foreignKey,
    Expression<Func<TEntity, object?>>? principalKey = null,
    bool isRequired = false,
    DeleteBehavior deleteBehavior = DeleteBehavior.ClientSetNull,
    Expression<Func<TEntity, IEnumerable<TDependent>?>>? inverseNavigation = null
) where TEntity : class where TDependent : class;
```
