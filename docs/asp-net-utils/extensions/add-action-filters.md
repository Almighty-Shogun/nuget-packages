---
returns: The `IServiceCollection` instance with MVC controllers and package action filters configured.
---

# AddActionFilters

Registers MVC controller services and adds the package `SessionContextFilter` as a global action filter. The filter captures request metadata before controller actions run and stores it in `HttpContext.Items` for later access through `GetSessionContext`.

Use this method when controllers or services need consistent access to request IP address and raw User-Agent data without repeating header parsing in every action. The method calls `AddControllers`, so it belongs in ASP.NET Core API startup code.

## Usage

```csharp
using AlmightyShogun.AspNet.Utils;

builder.Services.AddActionFilters();
```

<FrontmatterDocs/>

## Type signature

```csharp
public IServiceCollection AddActionFilters();
```
