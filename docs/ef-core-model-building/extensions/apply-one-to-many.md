---
params:
    - name: navigation
      description: Collection navigation on the principal entity.
      type: 'Expression<Func<TEntity, IEnumerable<TDependent>?>>'
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
      description: Reference navigation on the dependent back to the principal.
      type: 'Expression<Func<TDependent, TEntity?>>?'
      default: 'null'

returns: The same `ModelBuilder` instance.
---

# ApplyOneToMany

Configures a one-to-many relationship where `TEntity` is the principal holding a collection of `TDependent`.

The defaults describe an optional relationship: the foreign key is nullable and is cleared when the principal is deleted. Pass `isRequired: true` with `DeleteBehavior.Cascade` when the dependents cannot exist without it.

## Usage

::: code-group

```csharp [Optional.cs]
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.EntityFrameworkCore.ModelBuilding;

modelBuilder.ApplyOneToMany<Account, Order>(
    account => account.Orders,
    order => order.AccountId
);
```

```csharp [Required.cs]
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.EntityFrameworkCore.ModelBuilding;

modelBuilder.ApplyOneToMany<Account, Order>(
    account => account.Orders,
    order => order.AccountId,
    isRequired: true,
    deleteBehavior: DeleteBehavior.Cascade
);
```

```csharp [Entities.cs]
public sealed class Account
{
    public int Id { get; set; }
    public List<Order> Orders { get; set; } = [];
}

public sealed class Order
{
    public int Id { get; set; }
    public int? AccountId { get; set; }
}
```

:::

<FrontmatterDocs/>

## Type signature

```csharp
public ModelBuilder ApplyOneToMany<TEntity, TDependent>(
    Expression<Func<TEntity, IEnumerable<TDependent>?>> navigation,
    Expression<Func<TDependent, object?>> foreignKey,
    Expression<Func<TEntity, object?>>? principalKey = null,
    bool isRequired = false,
    DeleteBehavior deleteBehavior = DeleteBehavior.ClientSetNull,
    Expression<Func<TDependent, TEntity?>>? inverseNavigation = null
) where TEntity : class where TDependent : class;
```
