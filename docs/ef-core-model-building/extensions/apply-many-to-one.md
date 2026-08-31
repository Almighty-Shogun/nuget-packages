---
params:
    - name: navigation
      description: The reference property on the dependent. This produces the same model as writing the relationship from the collection side, foreign key included; only the side the call is written from differs.
      type: 'Expression<Func<TDependent, TEntity?>>'

    - name: foreignKey
      description: The property on the dependent holding the key. Its nullability is what decides whether the reference is optional, so a nullable key is how a dependent is allowed to stand alone.
      type: 'Expression<Func<TDependent, object?>>'

    - name: inverseNavigation
      description: The collection property on the principal. Leave it unset when the principal exposes no collection.
      type: 'Expression<Func<TEntity, IEnumerable<TDependent>?>>?'
      default: 'null'

    - name: principalKey
      description: On the four-argument overload only, the property on the principal the foreign key targets. EF Core promotes it to an alternate key, so it needs a unique index of its own.
      type: 'Expression<Func<TEntity, object?>>'

returns: The `ModelBuilder` instance with the relationship configured.
---

# ApplyManyToOne

Configures the same shape as [`ApplyOneToMany`](./apply-one-to-many), written from the dependent's side for a model where the reference reads better than the collection.

Requiredness and delete behavior are left to EF Core, which infers both from the foreign key property: a non-nullable key gives a required reference whose dependents are deleted with the principal, a nullable one gives an optional reference using `ClientSetNull`. That clears the key only on dependents EF is already tracking, and the database constraint it creates does not cascade, so deleting a principal whose dependents are not loaded fails at the database.

## Usage

::: code-group

```csharp [Required.cs]
using AlmightyShogun.EntityFrameworkCore.ModelBuilding;

modelBuilder.ApplyManyToOne<Account, Order>(
    order => order.Account,
    order => order.AccountId,
    account => account.Orders
);
```

```csharp [Optional.cs]
using AlmightyShogun.EntityFrameworkCore.ModelBuilding;

modelBuilder.ApplyManyToOne<Account, Badge>(
    badge => badge.Account,
    badge => badge.AccountId
);
```

```csharp [AlternateKey.cs]
using AlmightyShogun.EntityFrameworkCore.ModelBuilding;

modelBuilder.ApplyManyToOne<Account, Ticket>(
    ticket => ticket.Account,
    ticket => ticket.AccountReference,
    null,
    account => account.Reference
);
```

```csharp [Entities.cs]
public sealed class Account
{
    public int Id { get; set; }
    public Guid Reference { get; set; }
    public List<Order> Orders { get; set; } = [];
}

public sealed class Order
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public Account? Account { get; set; }
}

public sealed class Badge
{
    public int Id { get; set; }
    public int? AccountId { get; set; }
    public Account? Account { get; set; }
}

public sealed class Ticket
{
    public int Id { get; set; }
    public Guid AccountReference { get; set; }
    public Account? Account { get; set; }
}
```

:::

::: tip
The four-argument overload is for an alternate principal key and takes `inverseNavigation` explicitly, passing `null` when there is none, so the two overloads stay distinguishable.
:::

<FrontmatterDocs/>

## Type signature

```csharp
public ModelBuilder ApplyManyToOne<TEntity, TDependent>(
    Expression<Func<TDependent, TEntity?>> navigation,
    Expression<Func<TDependent, object?>> foreignKey,
    Expression<Func<TEntity, IEnumerable<TDependent>?>>? inverseNavigation = null
) where TEntity : class where TDependent : class;

public ModelBuilder ApplyManyToOne<TEntity, TDependent>(
    Expression<Func<TDependent, TEntity?>> navigation,
    Expression<Func<TDependent, object?>> foreignKey,
    Expression<Func<TEntity, IEnumerable<TDependent>?>>? inverseNavigation,
    Expression<Func<TEntity, object?>> principalKey
) where TEntity : class where TDependent : class;
```
