---
params:
    - name: navigation
      description: Reference navigation on the principal entity pointing to the dependent entity.
      type: 'Expression<Func<TEntity, TDependent?>>'

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
      default: 'true'

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

# ApplyOneToOne

Configures a one-to-one relationship where `TEntity` is the principal entity and `TDependent` is the dependent entity. The method starts from the principal entity, uses the provided reference navigation, configures the dependent foreign key, applies delete behavior, and optionally sets the inverse navigation and principal key.

Use this method when the principal entity has a single dependent reference and the dependent entity owns the foreign key. Pass `inverseNavigation` when the dependent entity also has a reference back to the principal entity. If it is omitted, EF Core configures the relationship as one-sided even when such a property exists on the dependent entity.

## Usage

::: code-group

```csharp [AppDbContext.cs]
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.EntityFrameworkCore.Utils;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    public DbSet<UserProfile> Profiles => Set<UserProfile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .ApplyOneToOne<User, UserProfile>(
                user => user.Profile,
                profile => profile.UserId,
                isRequired: true,
                deleteBehavior: DeleteBehavior.Cascade,
                inverseNavigation: profile => profile.User
            )
            .ApplyAutoInclude<User>(user => user.Profile);
    }
}
```

```csharp [Entities.cs]
public sealed class User
{
    public int Id { get; set; }

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
public ModelBuilder ApplyOneToOne<TEntity, TDependent>(
    Expression<Func<TEntity, TDependent?>> navigation,
    Expression<Func<TDependent, object?>> foreignKey,
    Expression<Func<TEntity, object?>>? principalKey = null,
    bool isRequired = true,
    DeleteBehavior deleteBehavior = DeleteBehavior.ClientSetNull,
    Expression<Func<TDependent, TEntity?>>? inverseNavigation = null
) where TEntity : class where TDependent : class;
```
