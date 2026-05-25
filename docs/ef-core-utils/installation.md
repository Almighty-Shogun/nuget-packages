# Installation

Install `AlmightyShogun.EntityFrameworkCore.Utils` in the project that contains the `DbContext` or entity model configuration. The package targets `net10.0` and adds extension methods for Entity Framework Core's `ModelBuilder`.

```sh
dotnet add package AlmightyShogun.EntityFrameworkCore.Utils
```

## Dependencies

- `Microsoft.EntityFrameworkCore` `10.0.8` &mdash; provides `ModelBuilder`, relationship builders, delete behavior options, and navigation configuration APIs used by the package.

## Startup Registration

This package does not register services. Call the extension methods from `DbContext.OnModelCreating` or from model configuration code that receives a `ModelBuilder`.

```csharp
using Microsoft.EntityFrameworkCore;
using AlmightyShogun.EntityFrameworkCore.Utils;

protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.ApplyAutoInclude<User>(user => user.Profile);
}
```
