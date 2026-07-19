# Installation

Install `AlmightyShogun.EntityFrameworkCore.Utils` in the project that contains the `DbContext` or entity model configuration. The package targets `net10.0` and adds extension methods for Entity Framework Core's `ModelBuilder`.

```sh
dotnet add package AlmightyShogun.EntityFrameworkCore.Utils
```

## Dependencies

### Package references

- `Microsoft.EntityFrameworkCore` `10.0.10` &mdash; provides `ModelBuilder`, relationship builders, delete behavior options, and navigation configuration APIs used by the package.

## Startup Registration

This package does not register services. Call the extension methods from `DbContext.OnModelCreating` or from model configuration code that receives a `ModelBuilder`. The methods return the same model builder instance, so related configuration can be chained when it belongs together.

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
