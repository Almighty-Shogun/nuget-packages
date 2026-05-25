---
outline: deep

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
---

# ApplyOneToMany

Configures a one-to-many relationship where `TEntity` is the principal entity and `TDependent` is the dependent entity. The method starts from the principal collection navigation, configures the dependent foreign key, applies delete behavior, and optionally sets a principal key.

Use this method when a principal entity owns a collection of dependents and the dependent entity stores the foreign key. The default relationship is optional unless `isRequired` is set to `true`.

## Usage

```csharp
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.EntityFrameworkCore.Utils;

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.ApplyOneToMany<User, UserSession>(
        user => user.Sessions,
        session => session.UserId,
        deleteBehavior: DeleteBehavior.Cascade
    );
}
```

<FrontmatterDocs/>

## Type signature

```csharp
public void ApplyOneToMany<TEntity, TDependent>(
    Expression<Func<TEntity, IEnumerable<TDependent>?>> navigation,
    Expression<Func<TDependent, object?>> foreignKey,
    Expression<Func<TEntity, object?>>? principalKey = null,
    bool isRequired = false,
    DeleteBehavior deleteBehavior = DeleteBehavior.ClientSetNull
) where TEntity : class where TDependent : class;
```
