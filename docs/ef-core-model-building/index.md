# Entity Framework Core Model Building

Model configuration helpers for `ModelBuilder`, covering the shapes written on almost every model: relationships, indexes, owned types, enum storage, and eager loading.

Each helper collapses a fluent-API sequence into one call without overriding a convention: requiredness and delete behavior stay inferred from the foreign key's nullability. Every one is called inside `OnModelCreating` and returns the `ModelBuilder`, so calls chain.

## Categories

- [Extensions](./extensions/apply-one-to-one) &mdash; model configuration helpers on `ModelBuilder`.

## Quick Example

::: code-group

```csharp [AppDbContext.cs]
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.EntityFrameworkCore.ModelBuilding;

public sealed class AppDbContext(
    DbContextOptions<AppDbContext> options
) : DbContext(options)
{
    public DbSet<Account> Accounts => Set<Account>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyOneToOne<Account, Profile>(
            account => account.Profile,
            profile => profile.AccountId
        );
        
        modelBuilder.ApplyOneToMany<Account, Order>(
            account => account.Orders,
            order => order.AccountId
        );

        modelBuilder.ApplyUniqueIndex<Account>(
            account => account.Email
        );

        modelBuilder.ApplyUniqueIndex<Account>(
            account => account.Slug,
            filter: "[Slug] IS NOT NULL"
        );
        
        modelBuilder.ApplyEnumAsString<Account, Tier>(
            account => account.Tier
        );
        
        modelBuilder.ApplyOwned<Account, Address>(
            account => account.BillingAddress,
            "Billing"
        );

        modelBuilder.ApplyManyToMany<Account, Tag>(
            account => account.Tags,
            tag => tag.Accounts,
            "account_tags"
        );
    }
}
```

```csharp [Account.cs]
public sealed class Account
{
    public int Id { get; set; }
    public required string Email { get; set; }
    public string? Slug { get; set; }
    public Tier Tier { get; set; }
    public Profile? Profile { get; set; }
    public Address? BillingAddress { get; set; }
    public List<Order> Orders { get; set; } = [];
    public List<Tag> Tags { get; set; } = [];
}
```

:::
