# ServiceCollectionExtensions

Extension methods for registering small service modules, strongly typed configuration, and discovered implementations with `IServiceCollection`. These helpers are intended for package startup code and applications that use dependency injection consistently.

Use these methods when a service can configure itself through `IService`, when options should be bound and validated from an `IConfigurationSection`, or when implementations should be discovered from assemblies instead of registered one by one.

## Methods

- [AddService](./add-service) &mdash; creates an `IService` module and lets it configure services.
- [AddConfiguration](./add-configuration) &mdash; binds and validates a strongly typed options class.
- [RegisterOnInherit](./register-on-inherit) &mdash; scans assemblies and registers concrete types assignable to `T`.
