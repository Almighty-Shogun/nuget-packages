# Entity Framework Core Utils

Adds small `ModelBuilder` extension methods for common Entity Framework Core relationship, navigation, and index configuration. The package focuses on reducing repeated fluent API code in `OnModelCreating` while still using EF Core's normal builders under the hood.

Use this package when a project repeatedly configures one-to-one, one-to-many, many-to-one, auto-included navigation properties, or simple indexes and wants those patterns to stay compact and consistent. Relationship helpers can map only the supplied navigation or include the inverse navigation when both sides exist on the entities.

## Categories

- [Extensions](./extensions/apply-auto-include) &mdash; public `ModelBuilder` extension methods for relationship, navigation, and index configuration.

## Quick Example

::: code-group

```csharp [AppDbContext.cs]
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.EntityFrameworkCore.Utils;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    public DbSet<UserSession> Sessions => Set<UserSession>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .ApplyOneToMany<User, UserSession>(
                user => user.Sessions,
                session => session.UserId,
                inverseNavigation: session => session.User
            )
            .ApplyIndex<UserSession>(session => session.UserId);
    }
}
```

```csharp [Entities.cs]
public sealed class User
{
    public int Id { get; set; }

    public List<UserSession> Sessions { get; set; } = [];
}

public sealed class UserSession
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public User? User { get; set; }
}
```

:::
