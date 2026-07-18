---
params:
    - name: navigation
      description: Reference navigation on the dependent entity pointing to the principal entity.
      type: 'Expression<Func<TDependent, TEntity?>>'

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
      description: Optional collection navigation on the principal entity containing dependent entities. When omitted, EF Core configures the relationship without an inverse navigation.
      type: 'Expression<Func<TEntity, IEnumerable<TDependent>?>>?'
      default: 'null'

returns: The model builder instance.
---

# ApplyManyToOne

Configures a many-to-one relationship from a dependent entity to a principal entity. The method starts from the dependent entity, uses the dependent reference navigation, configures the dependent foreign key, applies delete behavior, and optionally sets the principal inverse collection and principal key.

Use this method when the model code is written from the dependent side of the relationship. This is useful for entities such as sessions, orders, or audit records that each point back to one owning entity. Pass `inverseNavigation` when the principal entity also exposes a collection of dependents.

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
            .ApplyManyToOne<User, UserSession>(
                session => session.User,
                session => session.UserId,
                isRequired: true,
                inverseNavigation: user => user.Sessions
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
public ModelBuilder ApplyManyToOne<TEntity, TDependent>(
    Expression<Func<TDependent, TEntity?>> navigation,
    Expression<Func<TDependent, object?>> foreignKey,
    Expression<Func<TEntity, object?>>? principalKey = null,
    bool isRequired = false,
    DeleteBehavior deleteBehavior = DeleteBehavior.ClientSetNull,
    Expression<Func<TEntity, IEnumerable<TDependent>?>>? inverseNavigation = null
) where TEntity : class where TDependent : class;
```
