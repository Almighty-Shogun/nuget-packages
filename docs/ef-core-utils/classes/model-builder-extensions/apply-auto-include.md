---
outline: deep

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

```csharp
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.EntityFrameworkCore.Utils;

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder
        .ApplyAutoInclude<User>(user => user.Profile)
        .ApplyIndex<User>(user => user.Email, isUnique: true);
}
```

<FrontmatterDocs/>

## Type signature

```csharp
public ModelBuilder ApplyAutoInclude<TEntity>(
    Expression<Func<TEntity, object?>> navigation
) where TEntity : class;
```
