# Entity Framework Core Utils

Adds small `ModelBuilder` extension methods for common Entity Framework Core relationship, navigation, and index configuration. The package focuses on reducing repeated fluent API code in `OnModelCreating` while still using EF Core's normal builders under the hood.

Use this package when a project repeatedly configures one-to-one, one-to-many, many-to-one, auto-included navigation properties, or simple indexes and wants those patterns to stay compact and consistent. Relationship helpers can map only the supplied navigation or include the inverse navigation when both sides exist on the entities.

## Categories

- [Classes](./classes/model-builder-extensions/) &mdash; public `ModelBuilder` extension methods for relationship, navigation, and index configuration.

## Quick Example

```csharp
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.EntityFrameworkCore.Utils;

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
```
