---
outline: deep

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
---

# ApplyManyToOne

Configures a many-to-one relationship from a dependent entity to a principal entity. The method starts from the dependent entity, uses the dependent reference navigation, configures the dependent foreign key, applies delete behavior, and optionally sets a principal key.

Use this method when the model code is written from the dependent side of the relationship. This is useful for entities such as sessions, orders, or audit records that each point back to one owning entity.

## Usage

```csharp
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.EntityFrameworkCore.Utils;

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.ApplyManyToOne<User, UserSession>(
        session => session.User,
        session => session.UserId,
        isRequired: true
    );
}
```

<FrontmatterDocs/>

## Type signature

```csharp
public void ApplyManyToOne<TEntity, TDependent>(
    Expression<Func<TDependent, TEntity?>> navigation,
    Expression<Func<TDependent, object?>> foreignKey,
    Expression<Func<TEntity, object?>>? principalKey = null,
    bool isRequired = false,
    DeleteBehavior deleteBehavior = DeleteBehavior.ClientSetNull
) where TEntity : class where TDependent : class;
```
