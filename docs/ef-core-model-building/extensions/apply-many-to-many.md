---
params:
    - name: navigation
      description: The collection property on `TEntity`, whose join column is named after that type.
      type: 'Expression<Func<TEntity, IEnumerable<TRelated>?>>'

    - name: inverseNavigation
      description: The collection property on `TRelated`. Both sides are required, because a many-to-many with a navigation on one side only has no second collection for EF Core to pair the join rows with.
      type: 'Expression<Func<TRelated, IEnumerable<TEntity>?>>'

    - name: joinTableName
      description: The table holding the pairs. Named explicitly because EF Core's generated name concatenates the two entity names, which reads poorly in a migration and changes if either type is renamed.
      type: string

returns: The `ModelBuilder` instance with the relationship and its join table configured.
---

# ApplyManyToMany

Configures a many-to-many relationship over an explicitly named join table, whose columns are named `{TypeName}Id` after the two entities, so an `Account` and a `Tag` give `AccountId` and `TagId`.

Both join columns are non-nullable, so EF Core cascades from either side by convention and a join row disappears with whichever entity it referenced.

## Usage

::: code-group

```csharp [AppDbContext.cs]
using AlmightyShogun.EntityFrameworkCore.ModelBuilding;

modelBuilder.ApplyManyToMany<Account, Tag>(
    account => account.Tags,
    tag => tag.Accounts,
    "account_tags"
);
```

```csharp [Entities.cs]
public sealed class Account
{
    public int Id { get; set; }
    public List<Tag> Tags { get; set; } = [];
}

public sealed class Tag
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public List<Account> Accounts { get; set; } = [];
}
```

:::

::: warning
The join table carries nothing but the two keys, and its column names are fixed by the convention above. A join that stores data of its own, such as an `EnrolledAt` timestamp, or one that needs different column names, is past what this hides: configure it with `UsingEntity` directly.
:::

<FrontmatterDocs/>

## Type signature

```csharp
public ModelBuilder ApplyManyToMany<TEntity, TRelated>(
    Expression<Func<TEntity, IEnumerable<TRelated>?>> navigation,
    Expression<Func<TRelated, IEnumerable<TEntity>?>> inverseNavigation,
    string joinTableName
) where TEntity : class where TRelated : class;
```
