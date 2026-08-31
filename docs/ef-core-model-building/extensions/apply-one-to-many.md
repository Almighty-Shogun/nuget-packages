---
params:
    - name: navigation
      description: The collection property on the principal. Its element type decides which entity is expected to carry the foreign key, which is the one held in the collection rather than the one holding it.
      type: 'Expression<Func<TEntity, IEnumerable<TDependent>?>>'

    - name: foreignKey
      description: The property on the dependent holding the key. Its nullability is what decides whether a dependent may exist without a principal, and with it whether deleting the principal cascades or orphans the rows.
      type: 'Expression<Func<TDependent, object?>>'

    - name: inverseNavigation
      description: The property on the dependent pointing back at its principal. Leave it unset when the dependent has none.
      type: 'Expression<Func<TDependent, TEntity?>>?'
      default: 'null'

    - name: principalKey
      description: On the four-argument overload only, the property on the principal the foreign key targets. EF Core promotes it to an alternate key, so it needs a unique index of its own.
      type: 'Expression<Func<TEntity, object?>>'

returns: The `ModelBuilder` instance with the relationship configured.
---

# ApplyOneToMany

Configures a one-to-many relationship where `TEntity` holds a collection of `TDependent`.

Requiredness and delete behavior are left to EF Core, which infers both from the foreign key property: a non-nullable key gives a required relationship whose dependents are deleted with the principal, a nullable one gives an optional relationship using `ClientSetNull`. That clears the key only on dependents EF is already tracking, and the database constraint it creates does not cascade, so deleting a principal whose dependents are not loaded fails at the database.

## Usage

::: code-group

```csharp [Required.cs]
using AlmightyShogun.EntityFrameworkCore.ModelBuilding;

modelBuilder.ApplyOneToMany<Account, Order>(
    account => account.Orders,
    order => order.AccountId,
    order => order.Account
);
```

```csharp [Optional.cs]
using AlmightyShogun.EntityFrameworkCore.ModelBuilding;

modelBuilder.ApplyOneToMany<Account, Badge>(
    account => account.Badges,
    badge => badge.AccountId
);
```

```csharp [AlternateKey.cs]
using AlmightyShogun.EntityFrameworkCore.ModelBuilding;

modelBuilder.ApplyOneToMany<Account, Ticket>(
    account => account.Tickets,
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
    public List<Badge> Badges { get; set; } = [];
    public List<Ticket> Tickets { get; set; } = [];
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
}

public sealed class Ticket
{
    public int Id { get; set; }
    public Guid AccountReference { get; set; }
}
```

:::

::: tip
The four-argument overload is for an alternate principal key and takes `inverseNavigation` explicitly, passing `null` when there is none, so the two overloads stay distinguishable.
:::

<FrontmatterDocs/>

## Type signature

```csharp
public ModelBuilder ApplyOneToMany<TEntity, TDependent>(
    Expression<Func<TEntity, IEnumerable<TDependent>?>> navigation,
    Expression<Func<TDependent, object?>> foreignKey,
    Expression<Func<TDependent, TEntity?>>? inverseNavigation = null
) where TEntity : class where TDependent : class;

public ModelBuilder ApplyOneToMany<TEntity, TDependent>(
    Expression<Func<TEntity, IEnumerable<TDependent>?>> navigation,
    Expression<Func<TDependent, object?>> foreignKey,
    Expression<Func<TDependent, TEntity?>>? inverseNavigation,
    Expression<Func<TEntity, object?>> principalKey
) where TEntity : class where TDependent : class;
```
