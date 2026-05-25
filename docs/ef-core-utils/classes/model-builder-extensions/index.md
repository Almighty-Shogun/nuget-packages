# ModelBuilderExtensions

Contains extension methods for configuring common Entity Framework Core model relationships and navigation behavior. The methods are intended to be called from `OnModelCreating` and keep repeated fluent API chains in one place.

Use these extensions when a relationship follows the package's supported shapes and the default `WithOne()` or `WithMany()` side is enough for the model. If a relationship needs a custom inverse navigation, additional constraints, or advanced EF Core mapping behavior, use EF Core's fluent API directly for that case.

## Usage

```csharp
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.EntityFrameworkCore.Utils;

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.ApplyManyToOne<User, UserSession>(
        session => session.User,
        session => session.UserId
    );
}
```

## Methods

- [ApplyAutoInclude](./apply-auto-include) &mdash; configures a navigation to be automatically included in queries.
- [ApplyManyToOne](./apply-many-to-one) &mdash; configures a dependent-to-principal reference relationship.
- [ApplyOneToMany](./apply-one-to-many) &mdash; configures a principal-to-dependent collection relationship.
- [ApplyOneToOne](./apply-one-to-one) &mdash; configures a principal-to-dependent reference relationship.
