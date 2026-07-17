---
outline: deep

params:
    - name: index
      description: Property expression, or anonymous-object expression, that identifies the indexed property or composite index properties.
      type: 'Expression<Func<TEntity, object?>>'

    - name: isUnique
      description: Whether EF Core should configure the index as unique.
      type: bool
      default: 'false'

returns: The model builder instance.
---

# ApplyIndex

Configures an EF Core index for an entity type. The method starts from `ModelBuilder.Entity<TEntity>()`, calls EF Core's index builder with the provided expression, and applies the requested uniqueness setting.

Use this method when model configuration needs a compact, chainable index declaration alongside the package relationship helpers. Pass a single property for a normal index, or an anonymous object when the index should cover multiple properties. If the index needs a database name, filter, sort order, or provider-specific options, use EF Core's fluent API directly for that mapping.

## Usage

```csharp
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.EntityFrameworkCore.Utils;

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder
        .ApplyIndex<User>(user => user.Email, isUnique: true)
        .ApplyIndex<UserSession>(session => new { session.UserId, session.CreatedAt });
}
```

<FrontmatterDocs/>

## Type signature

```csharp
public ModelBuilder ApplyIndex<TEntity>(
    Expression<Func<TEntity, object?>> index,
    bool isUnique = false
) where TEntity : class;
```
