# Installation

Install `AlmightyShogun.EntityFrameworkCore.ModelBuilding` in the project that owns the `DbContext`, which is the project where `OnModelCreating` is written. The package targets `net10.0`.

```sh
dotnet add package AlmightyShogun.EntityFrameworkCore.ModelBuilding
```

## Dependencies

### Package references

- `Microsoft.EntityFrameworkCore` `10.0.11` &mdash; supplies `ModelBuilder`, the type every helper extends.
- `Microsoft.EntityFrameworkCore.Relational` `10.0.11` &mdash; supplies index names, index filters, and owned-type column naming.

## Usage

The package registers no services and has no startup call. The helpers are extension methods on `ModelBuilder`, available inside `OnModelCreating` once the namespace is imported:

```csharp
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.EntityFrameworkCore.ModelBuilding;

public sealed class AppDbContext(
    DbContextOptions<AppDbContext> options
) : DbContext(options)
{
    public DbSet<Account> Accounts => Set<Account>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyOneToMany<Account, Order>(
            account => account.Orders,
            order => order.AccountId
        );
        
        modelBuilder.ApplyUniqueIndex<Account>(
            account => account.Email
        );
    }
}
```

Every helper returns the same `ModelBuilder`, so calls can be chained or written one per line. Configuration applied through a helper can still be extended afterwards with the standard fluent API on the same entity.

Adding or changing a helper call changes the model, so generate a migration afterwards:

```sh
dotnet ef migrations add AddAccountEmailIndex
```

## Provider support

The relational dependency means these helpers assume a relational provider: SQL Server, PostgreSQL, MySQL, MariaDB, SQLite, and equivalents.

Two parameters carry provider-specific behavior:

- `filter` on [`ApplyUniqueIndex`](./extensions/apply-unique-index) is raw SQL, and identifier quoting differs per provider: `[Slug]` on SQL Server, `"Slug"` on PostgreSQL and SQLite, `` `Slug` `` on MySQL and MariaDB.
- `columnPrefix` on [`ApplyOwned`](./extensions/apply-owned) renames physical columns.

On a document provider such as Cosmos, those two have no effect and the relationship helpers do not apply.
