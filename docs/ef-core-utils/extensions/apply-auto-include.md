---
params:
    - name: navigation
      description: Navigation property that EF Core should automatically include whenever the entity is queried.
      type: 'Expression<Func<TEntity, object?>>'

returns: The model builder instance.
---

# ApplyAutoInclude

Configures an entity navigation to be automatically included in EF Core queries. The method calls EF Core's navigation configuration for `TEntity` and enables `AutoInclude` on the provided navigation expression.

Use this method for navigations that should almost always be loaded with the entity. Avoid applying it to large collections or rarely needed navigations, because automatic includes affect every query for the entity unless explicitly ignored. The method returns the same `ModelBuilder` instance for fluent model configuration.

## Usage

::: code-group

```csharp [AppDbContext.cs]
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.EntityFrameworkCore.Utils;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .ApplyAutoInclude<User>(user => user.Profile)
            .ApplyIndex<User>(user => user.Email, isUnique: true);
    }
}
```

```csharp [Entities.cs]
public sealed class User
{
    public int Id { get; set; }

    public string Email { get; set; } = "";

    public UserProfile? Profile { get; set; }
}

public sealed class UserProfile
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public User? User { get; set; }
}
```

:::

<FrontmatterDocs/>

## Type signature

```csharp
public ModelBuilder ApplyAutoInclude<TEntity>(
    Expression<Func<TEntity, object?>> navigation
) where TEntity : class;
```
