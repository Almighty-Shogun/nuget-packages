---
params:
    - name: index
      description: The property or property set to index.
      type: 'Expression<Func<TEntity, object?>>'
    - name: isUnique
      description: Whether the index enforces uniqueness.
      type: bool
      default: 'false'
    - name: databaseName
      description: Index name in the database. Set it to keep the name stable across migrations instead of using the generated one, which changes whenever the indexed columns change.
      type: string?
      default: 'null'
    - name: filter
      description: SQL filter expression limiting the index to matching rows. A unique index needs one to tolerate multiple nulls, which some providers otherwise treat as equal and reject.
      type: string?
      default: 'null'

returns: The same `ModelBuilder` instance.
---

# ApplyIndex

Configures an index on one property or a set of properties, optionally unique, explicitly named, and filtered. A unique index without a filter treats nulls as equal on most providers, so two rows with a null value collide. Naming it keeps the name stable across migrations, where a generated name changes whenever the indexed columns do.

## Usage

::: code-group

```csharp [Simple.cs]
using AlmightyShogun.EntityFrameworkCore.Utils;

modelBuilder.ApplyIndex<Account>(account => account.Email, isUnique: true);
```

```csharp [Composite.cs]
using AlmightyShogun.EntityFrameworkCore.Utils;

modelBuilder.ApplyIndex<Order>(order => new 
{
    order.AccountId,
    order.PlacedAt
});
```

```csharp [Named.cs]
using AlmightyShogun.EntityFrameworkCore.Utils;

modelBuilder.ApplyIndex<Account>(
    account => account.Email,
    isUnique: true,
    databaseName: "ix_accounts_email"
);
```

```csharp [Filtered.cs]
using AlmightyShogun.EntityFrameworkCore.Utils;

modelBuilder.ApplyIndex<Account>(
    account => account.Slug,
    isUnique: true,
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
public ModelBuilder ApplyIndex<TEntity>(
    Expression<Func<TEntity, object?>> index,
    bool isUnique = false,
    string? databaseName = null,
    string? filter = null
) where TEntity : class;
```
