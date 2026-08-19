---
params:
    - name: navigation
      description: Reference navigation on the principal entity.
      type: 'Expression<Func<TEntity, TDependent?>>'
    - name: foreignKey
      description: Foreign key property on the dependent entity.
      type: 'Expression<Func<TDependent, object?>>'
    - name: principalKey
      description: Principal key property. When omitted, the principal's primary key is used.
      type: 'Expression<Func<TEntity, object?>>?'
      default: 'null'
    - name: isRequired
      description: Whether the dependent must always have a principal.
      type: bool
      default: 'true'
    - name: deleteBehavior
      description: What happens to the dependent when the principal is deleted.
      type: DeleteBehavior
      default: DeleteBehavior.Cascade
    - name: inverseNavigation
      description: Reference navigation on the dependent back to the principal. When omitted, the relationship has no inverse navigation.
      type: 'Expression<Func<TDependent, TEntity?>>?'
      default: 'null'

returns: The same `ModelBuilder` instance.
---

# ApplyOneToOne

Configures a one-to-one relationship where `TEntity` is the principal and `TDependent` is the dependent.

The defaults describe a required relationship: the dependent cannot exist without its principal, and deleting the principal deletes it. For an optional one, pass `isRequired: false` with `DeleteBehavior.ClientSetNull`.

## Usage

::: code-group

```csharp [AppDbContext.cs]
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.EntityFrameworkCore.ModelBuilding;

modelBuilder.ApplyOneToOne<Account, Profile>(
    account => account.Profile,
    profile => profile.AccountId
);
```

```csharp [Optional.cs]
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.EntityFrameworkCore.ModelBuilding;

modelBuilder.ApplyOneToOne<Account, Profile>(
    account => account.Profile,
    profile => profile.AccountId,
    isRequired: false,
    deleteBehavior: DeleteBehavior.ClientSetNull
);
```

```csharp [Entities.cs]
public sealed class Account
{
    public int Id { get; set; }
    public Profile? Profile { get; set; }
}

public sealed class Profile
{
    public int Id { get; set; }
    public int AccountId { get; set; }
}
```

:::

::: warning
`DeleteBehavior.ClientSetNull` on a required relationship is not a valid combination. It clears the dependent's foreign key, which a non-nullable column rejects, so deleting a principal fails at `SaveChanges` when the dependent is loaded and leaves an orphan or violates a database constraint when it is not.
:::

<FrontmatterDocs/>

## Type signature

```csharp
public ModelBuilder ApplyOneToOne<TEntity, TDependent>(
    Expression<Func<TEntity, TDependent?>> navigation,
    Expression<Func<TDependent, object?>> foreignKey,
    Expression<Func<TEntity, object?>>? principalKey = null,
    bool isRequired = true,
    DeleteBehavior deleteBehavior = DeleteBehavior.Cascade,
    Expression<Func<TDependent, TEntity?>>? inverseNavigation = null
) where TEntity : class where TDependent : class;
```
