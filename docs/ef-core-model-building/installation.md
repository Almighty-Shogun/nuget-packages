# Installation

Install `AlmightyShogun.EntityFrameworkCore.ModelBuilding` in the project that owns the `DbContext`, which is the project where `OnModelCreating` is written. The package targets `net10.0`.

```sh
dotnet add package AlmightyShogun.EntityFrameworkCore.ModelBuilding
```

## Dependencies

### Package references

- `Microsoft.EntityFrameworkCore` `10.0.11` &mdash; supplies `ModelBuilder`, the type every helper extends.
- `Microsoft.EntityFrameworkCore.Relational` `10.0.11` &mdash; supplies the index filter behind [`ApplyUniqueIndex`](./extensions/apply-unique-index).

## Usage

The package registers no services and has no startup call. The helpers are extension methods on `ModelBuilder`, available inside `OnModelCreating` once the namespace is imported, and what one configures can still be extended afterwards with the standard fluent API on the same entity.

::: tip
A helper call is model configuration like any other, so add a migration with `dotnet ef migrations add <Name>` after adding or changing one.
:::

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

## Provider support

Only one thing here is relational: the `filter` argument on [`ApplyUniqueIndex`](./extensions/apply-unique-index), which is raw SQL whose identifier quoting differs per provider. That argument is why the package takes the relational dependency; everything else uses provider-agnostic Entity Framework Core APIs.

On a document provider such as Cosmos, how far these relationship shapes apply is that provider's own business. The helpers do no provider branching of their own.
