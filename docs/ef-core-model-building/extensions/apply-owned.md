---
params:
    - name: navigation
      description: The navigation to the owned value.
      type: 'Expression<Func<TEntity, TOwned?>>'
    - name: columnPrefix
      description: Prefix applied to every owned column. Required when one entity owns two values of the same type, because their columns would otherwise collide.
      type: string?
      default: 'null'

returns: The same `ModelBuilder` instance.
---

# ApplyOwned

Configures a property as an owned type, stored in the owner's table rather than a separate one.

Use it for a value that belongs to exactly one entity and has no identity of its own, such as an address, a money amount with its currency, or a date range. An entity holding two owned values of the same type needs `columnPrefix` on at least one of them, otherwise their columns collide.

## Usage

::: code-group

```csharp [Single.cs]
using AlmightyShogun.EntityFrameworkCore.ModelBuilding;

modelBuilder.ApplyOwned<Account, Address>(
    account => account.BillingAddress
);
```

```csharp [Two.cs]
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
    string? columnPrefix = null
) where TEntity : class where TOwned : class;
```
