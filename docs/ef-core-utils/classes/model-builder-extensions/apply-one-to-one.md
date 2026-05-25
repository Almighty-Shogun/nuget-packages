---
outline: deep

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
---

# ApplyOneToOne

Configures a one-to-one relationship where `TEntity` is the principal entity and `TDependent` is the dependent entity. The method starts from the principal entity, uses the provided reference navigation, configures the dependent foreign key, applies delete behavior, and optionally sets a principal key.

Use this method when the principal entity has a single dependent reference and the dependent entity owns the foreign key. For relationship shapes with custom inverse navigations or additional constraints, use EF Core's fluent API directly.

## Usage

```csharp
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.EntityFrameworkCore.Utils;

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.ApplyOneToOne<User, UserProfile>(
        user => user.Profile,
        profile => profile.UserId,
        isRequired: true,
        deleteBehavior: DeleteBehavior.Cascade
    );
}
```

<FrontmatterDocs/>

## Type signature

```csharp
public void ApplyOneToOne<TEntity, TDependent>(
    Expression<Func<TEntity, TDependent?>> navigation,
    Expression<Func<TDependent, object?>> foreignKey,
    Expression<Func<TEntity, object?>>? principalKey = null,
    bool isRequired = true,
    DeleteBehavior deleteBehavior = DeleteBehavior.ClientSetNull
) where TEntity : class where TDependent : class;
```
