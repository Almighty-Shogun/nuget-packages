# ModelBuilderExtensions

Contains extension methods for configuring common Entity Framework Core model relationships, navigation behavior, and indexes. The relationship helpers support both one-sided mappings and bidirectional mappings with inverse navigations.

Use these extensions when a relationship follows the package's supported shapes and the default EF Core relationship builder is enough for the model. The methods return the same `ModelBuilder` instance, so several model rules can be chained without breaking out into separate statements. If a relationship or index needs additional constraints, a database-specific index option, or advanced EF Core mapping behavior, use EF Core's fluent API directly for that case.

## Usage

```csharp
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.EntityFrameworkCore.Utils;

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder
        .ApplyManyToOne<User, UserSession>(
            session => session.User,
            session => session.UserId,
            inverseNavigation: user => user.Sessions
        )
        .ApplyIndex<UserSession>(session => session.UserId);
}
```

## Methods

- [ApplyAutoInclude](./apply-auto-include) &mdash; configures a navigation to be automatically included in queries.
- [ApplyIndex](./apply-index) &mdash; configures a single-property or composite EF Core index.
- [ApplyManyToOne](./apply-many-to-one) &mdash; configures a dependent-to-principal reference relationship with an optional principal collection.
- [ApplyOneToMany](./apply-one-to-many) &mdash; configures a principal-to-dependent collection relationship with an optional dependent reference.
- [ApplyOneToOne](./apply-one-to-one) &mdash; configures a principal-to-dependent reference relationship with an optional dependent reference.
