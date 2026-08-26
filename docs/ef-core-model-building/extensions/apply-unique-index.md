---
params:
    - name: index
      description: The property to index, or an anonymous object of properties for a composite index. A composite unique index constrains the combination, not each column on its own.
      type: 'Expression<Func<TEntity, object?>>'

    - name: filter
      description: A provider-specific predicate limiting which rows the constraint covers. Worth setting over a nullable column, because several providers treat two nulls as equal and refuse the second row without it.
      type: string?
      default: 'null'

returns: The `ModelBuilder` instance with the unique index configured.
---

# ApplyUniqueIndex

Adds a unique index, which is how a value is kept unique in the database rather than only in the code that writes it. Use [`ApplyIndex`](./apply-index) for an index that only serves queries.

## Usage

::: code-group

```csharp [Single.cs]
using AlmightyShogun.EntityFrameworkCore.ModelBuilding;

modelBuilder.ApplyUniqueIndex<Account>(account => account.Email);
```

```csharp [Composite.cs]
using AlmightyShogun.EntityFrameworkCore.ModelBuilding;

modelBuilder.ApplyUniqueIndex<Membership>(membership => new
{
    membership.AccountId,
    membership.TeamId
});
```

```csharp [Filtered.cs]
using AlmightyShogun.EntityFrameworkCore.ModelBuilding;

modelBuilder.ApplyUniqueIndex<Account>(
    account => account.Slug,
    filter: "[Slug] IS NOT NULL"
);
```

:::

::: warning
`filter` is raw SQL and its identifier quoting is provider-specific: `[Slug]` on SQL Server, `"Slug"` on PostgreSQL and SQLite, `` `Slug` `` on MySQL and MariaDB.
:::

<FrontmatterDocs/>

## Type signature

```csharp
public ModelBuilder ApplyUniqueIndex<TEntity>(
    Expression<Func<TEntity, object?>> index,
    string? filter = null
) where TEntity : class;
```
