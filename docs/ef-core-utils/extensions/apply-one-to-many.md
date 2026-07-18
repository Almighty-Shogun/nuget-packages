---
params:
    - name: navigation
      description: Collection navigation on the principal entity containing dependent entities.
      type: 'Expression<Func<TEntity, IEnumerable<TDependent>?>>'

    - name: foreignKey
      description: Foreign key property on the dependent entity.
      type: 'Expression<Func<TDependent, object?>>'

    - name: principalKey
      description: Optional principal key property. When omitted, EF Core uses the principal primary key.
      type: 'Expression<Func<TEntity, object?>>?'
      default: 'null'

    - name: isRequired
      description: Whether the dependent relationship is required.
      type: bool
      default: 'false'

    - name: deleteBehavior
      description: Delete behavior applied to the relationship.
      type: DeleteBehavior
      default: DeleteBehavior.ClientSetNull

    - name: inverseNavigation
      description: Optional reference navigation on the dependent entity back to the principal entity. When omitted, EF Core configures the relationship without an inverse navigation.
      type: 'Expression<Func<TDependent, TEntity?>>?'
      default: 'null'

returns: The model builder instance.
---

# ApplyOneToMany

Configures a one-to-many relationship where `TEntity` is the principal entity and `TDependent` is the dependent entity. The method starts from the principal collection navigation, configures the dependent foreign key, applies delete behavior, and optionally sets the dependent inverse navigation and principal key.

Use this method when a principal entity owns a collection of dependents and the dependent entity stores the foreign key. Pass `inverseNavigation` when each dependent also has a reference back to the principal entity. If it is omitted, EF Core configures the collection side only.

## Usage

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
                deleteBehavior: DeleteBehavior.Cascade,
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

<FrontmatterDocs/>

## Type signature

```csharp
public ModelBuilder ApplyOneToMany<TEntity, TDependent>(
    Expression<Func<TEntity, IEnumerable<TDependent>?>> navigation,
    Expression<Func<TDependent, object?>> foreignKey,
    Expression<Func<TEntity, object?>>? principalKey = null,
    bool isRequired = false,
    DeleteBehavior deleteBehavior = DeleteBehavior.ClientSetNull,
    Expression<Func<TDependent, TEntity?>>? inverseNavigation = null
) where TEntity : class where TDependent : class;
```
