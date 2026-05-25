# Entity Framework Core Utils

Adds small `ModelBuilder` extension methods for common Entity Framework Core relationship configuration. The package focuses on reducing repeated fluent API code in `OnModelCreating` while still using EF Core's normal relationship builders under the hood.

Use this package when a project repeatedly configures one-to-one, one-to-many, many-to-one, or auto-included navigation properties and wants those patterns to stay compact and consistent.

## Categories

- [Classes](./classes/model-builder-extensions/) &mdash; public `ModelBuilder` extension methods for relationship and navigation configuration.

## Quick Example

```csharp
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.EntityFrameworkCore.Utils;

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.ApplyOneToMany<User, UserSession>(
        user => user.Sessions,
        session => session.UserId
    );
}
```
