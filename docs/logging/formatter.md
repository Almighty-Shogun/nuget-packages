# Formatter

The package always registers its internal console formatter when `AddCustomLogging` is used. Application code does not instantiate the formatter directly; write normal Serilog message templates and the registered logger applies the formatter to console output.

The formatter writes a timestamp, a three-letter log level, message-template text, colored property values, and exception details. Scalar values get default colors by type: strings are white, numbers are cyan, booleans are magenta, and `null` values are dark gray.

## Usage

```csharp
using Serilog;

Log.Information(
    "Processed {Count:c} items for {Application:bg} in {Elapsed:0.00|y} ms",
    42,
    "admin",
    18.742
);
```

Message-template property formats can include color shorthand. Use the shorthand as the property format, or combine a numeric format and color with `|`.

```csharp
using Serilog;

Log.Information("User {UserId:y} logged in", 42);
Log.Information("Completed in {Elapsed:0.00|g} ms", 18.742);
```

## Colors

| Code | Color |
| --- | --- |
| `r` | Red |
| `g` | Green |
| `b` | Blue |
| `c` | Cyan |
| `y` | Yellow |
| `m` | Magenta |
| `br` | Bright red |
| `bg` | Bright green |
| `bb` | Bright blue |
| `bc` | Bright cyan |
| `by` | Bright yellow |
| `bm` | Bright magenta |
