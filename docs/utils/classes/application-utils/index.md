# ApplicationUtils

Static utility class for small application-level helpers. It includes console window behavior and reflection-based type discovery used by packages that register classes from assemblies.

Use these helpers when the application needs to set console state, suppress Ctrl+C shutdown in an interactive process, or discover all loaded types that inherit from a base class or implement an interface.

## Methods

- [Title](./title) &mdash; sets the console window title.
- [GetOnInherit](./get-on-inherit) &mdash; finds types assignable to a base class or interface.
- [PreventCancellation](./prevent-cancellation) &mdash; cancels the default Ctrl+C shutdown behavior.
