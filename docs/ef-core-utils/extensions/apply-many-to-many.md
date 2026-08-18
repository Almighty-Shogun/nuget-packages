---
params:
    - name: navigation
      description: Collection navigation on `TEntity` to the related entities.
      type: 'Expression<Func<TEntity, IEnumerable<TRelated>?>>'
    - name: inverseNavigation
      description: Collection navigation on `TRelated` back to the first entities.
      type: 'Expression<Func<TRelated, IEnumerable<TEntity>?>>'
    - name: joinTableName
      description: The name of the join table.
      type: string
    - name: foreignKey
      description: Join column pointing at `TEntity`. Defaults to `{TEntity}Id`, so an `Account` produces `AccountId`.
      type: string?
      default: 'null'
    - name: relatedForeignKey
      description: Join column pointing at `TRelated`. Defaults to `{TRelated}Id`, so a `Tag` produces `TagId`.
      type: string?
      default: 'null'
    - name: deleteBehavior
      description: Delete behavior for both sides of the join.
      type: DeleteBehavior
      default: DeleteBehavior.Cascade

returns: The same `ModelBuilder` instance.
---

# ApplyManyToMany

Configures a many-to-many relationship through an implicit join table with no extra columns.

The two navigations are supplied in the order the type parameters declare them, and the helper derives both join columns from that order. Map the join entity yourself when the link needs columns of its own, such as a timestamp or a role.

## Usage

::: code-group

```csharp [AppDbContext.cs]
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.EntityFrameworkCore.Utils;

modelBuilder.ApplyManyToMany<Account, Tag>(
    account => account.Tags,
    tag => tag.Accounts,
    "account_tags"
);
```

```csharp [NamedColumns.cs]
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.EntityFrameworkCore.Utils;

modelBuilder.ApplyManyToMany<Account, Tag>(
    account => account.Tags,
    tag => tag.Accounts,
    "account_tags",
    foreignKey: "account_id",
    relatedForeignKey: "tag_id"
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
The join table cannot carry its own columns. A join that stores data of its own, such as an `EnrolledAt` timestamp, needs a real entity type; configure it with `UsingEntity<TJoin>` directly instead.
:::

<FrontmatterDocs/>

## Type signature

```csharp
public ModelBuilder ApplyManyToMany<TEntity, TRelated>(
    Expression<Func<TEntity, IEnumerable<TRelated>?>> navigation,
    Expression<Func<TRelated, IEnumerable<TEntity>?>> inverseNavigation,
    string joinTableName,
    string? foreignKey = null,
    string? relatedForeignKey = null,
    DeleteBehavior deleteBehavior = DeleteBehavior.Cascade
) where TEntity : class where TRelated : class;
```
