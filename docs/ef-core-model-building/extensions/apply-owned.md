---
params:
    - name: navigation
      description: The property on the owner holding the owned value.
      type: 'Expression<Func<TEntity, TOwned?>>'
    - name: columnPrefix
      description: The string put in front of every non-key column name, such as `"Billing"` giving `BillingStreet`. Required, because prefixing is the whole of what this adds over calling `OwnsOne` directly.
      type: string

returns: The `ModelBuilder` instance with the owned type mapped.
---

# ApplyOwned

Configures a property as an owned type, stored in the owner's table rather than a separate one.

Use it for a value that belongs to exactly one entity and has no identity of its own, such as an address, a money amount with its currency, or a date range. Every non-key column is prefixed, which is what lets one entity own two values of the same type without their columns colliding. Call `OwnsOne` directly when no prefix is wanted.

## Usage

::: code-group

```csharp [AppDbContext.cs]
using AlmightyShogun.EntityFrameworkCore.ModelBuilding;

modelBuilder.ApplyOwned<Account, Address>(
    account => account.BillingAddress,
    "Billing"
);

modelBuilder.ApplyOwned<Account, Address>(
    account => account.ShippingAddress,
    "Shipping"
);
```

```csharp [Entities.cs]
public sealed class Account
{
    public int Id { get; set; }
    public Address? BillingAddress { get; set; }
    public Address? ShippingAddress { get; set; }
}

public sealed class Address
{
    public required string City { get; set; }
    public required string Country { get; set; }
}
```

:::

## Querying

An owned value is part of its owner's row, so it loads with the owner and needs no `Include`:

```csharp
using Microsoft.EntityFrameworkCore;

Account? account = await database.Accounts.FirstOrDefaultAsync(
    candidate => candidate.Id == accountId,
    cancellationToken
);

string? city = account?.BillingAddress?.City;
```

Filtering and projecting through the navigation both translate to SQL, because the columns are on the same table:

```csharp
using Microsoft.EntityFrameworkCore;

List<string> cities = await database.Accounts
    .Where(account => account.BillingAddress!.Country == "NL")
    .Select(account => account.BillingAddress!.City)
    .ToListAsync(cancellationToken);
```

::: warning
There is no `DbSet` for an owned type and it cannot be the root of a query. `database.Set<Address>()` throws `InvalidOperationException`. Reach the value through its owner.
:::

<FrontmatterDocs/>

## Type signature

```csharp
public ModelBuilder ApplyOwned<TEntity, TOwned>(
    Expression<Func<TEntity, TOwned?>> navigation,
    string columnPrefix
) where TEntity : class where TOwned : class;
```
