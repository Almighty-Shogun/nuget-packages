---
params:
    - name: navigation
      description: The property on the principal that reaches the dependent. Which side declares it is what makes that side the principal, so naming the wrong one puts the foreign key on the wrong table.
      type: 'Expression<Func<TEntity, TDependent?>>'

    - name: foreignKey
      description: The property on the dependent holding the key. Its nullability is what decides whether the relationship is required, so make it non-nullable for a dependent that must always have a principal.
      type: 'Expression<Func<TDependent, object?>>'

    - name: inverseNavigation
      description: The property on the dependent pointing back. Leave it unset when the dependent has no such property, which EF Core maps as a one-directional relationship rather than as an error.
      type: 'Expression<Func<TDependent, TEntity?>>?'
      default: 'null'

    - name: principalKey
      description: On the four-argument overload only, the property on the principal the foreign key targets. EF Core promotes it to an alternate key, so it needs a unique index of its own.
      type: 'Expression<Func<TEntity, object?>>'

returns: The `ModelBuilder` instance with the relationship configured.
---

# ApplyOneToOne

Configures a one-to-one relationship where `TEntity` is the principal and `TDependent` carries the foreign key.

Requiredness and delete behavior are left to EF Core, which infers both from the foreign key property: make it non-nullable for a dependent that cannot exist alone and is deleted with its principal, nullable for one that can.

## Usage

::: code-group

```csharp [AppDbContext.cs]
using AlmightyShogun.EntityFrameworkCore.ModelBuilding;

modelBuilder.ApplyOneToOne<Account, Profile>(
    account => account.Profile,
    profile => profile.AccountId
);
```

```csharp [Bidirectional.cs]
using AlmightyShogun.EntityFrameworkCore.ModelBuilding;

modelBuilder.ApplyOneToOne<Account, Profile>(
    account => account.Profile,
    profile => profile.AccountId,
    profile => profile.Account
);
```

```csharp [AlternateKey.cs]
using AlmightyShogun.EntityFrameworkCore.ModelBuilding;

modelBuilder.ApplyOneToOne<Account, Profile>(
    account => account.Profile,
    profile => profile.AccountReference,
    null,
    account => account.Reference
);
```

```csharp [Entities.cs]
public sealed class Account
{
    public int Id { get; set; }
    public Guid Reference { get; set; }
    public Profile? Profile { get; set; }
}

public sealed class Profile
{
    public int Id { get; set; }
    public int AccountId { get; set; }
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
public ModelBuilder ApplyOneToOne<TEntity, TDependent>(
    Expression<Func<TEntity, TDependent?>> navigation,
    Expression<Func<TDependent, object?>> foreignKey,
    Expression<Func<TDependent, TEntity?>>? inverseNavigation = null
) where TEntity : class where TDependent : class;

public ModelBuilder ApplyOneToOne<TEntity, TDependent>(
    Expression<Func<TEntity, TDependent?>> navigation,
    Expression<Func<TDependent, object?>> foreignKey,
    Expression<Func<TDependent, TEntity?>>? inverseNavigation,
    Expression<Func<TEntity, object?>> principalKey
) where TEntity : class where TDependent : class;
```
