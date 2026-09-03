# Types and Files Rules

Type and file rules validate the runtime shape of the bound value. They are commonly used before content-specific validation so clients receive clear type or file errors instead of later rule failures.

## String

Requires the value to be a string.

::: code-group

```csharp [Attribute.cs]
[String]
```

```csharp [FluentRule.cs]
RuleFor(x => x.DisplayName)
    .String();
```

:::

## Array

Requires the value to be an enumerable value that is not a string.

::: code-group

```csharp [Attribute.cs]
[Array]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Tags)
    .Array();
```

:::

## List

Requires the value to be an array or list-like value.

::: code-group

```csharp [Attribute.cs]
[List]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Items)
    .List();
```

:::

## Boolean

Requires the value to be a boolean value.

::: code-group

```csharp [Attribute.cs]
[Boolean]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Enabled)
    .Boolean();
```

:::

## Enum

Requires the value to name a member the enum actually defines. The enum's own type, a case-sensitive member name, and an integral value all qualify; anything else fails rather than being converted first, so a `bool` or a fractional number does not pass as the member its conversion would land on. When the enum type is omitted, the validator uses the request property type.

::: code-group

```csharp [Attribute.cs]
[Enum(Type? enumType = null)]
[Enum<TEnum>]

[Enum]
[Enum(typeof(DayOfWeek))]
[Enum<OrderStatus>]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Status)
    .Enum<OrderStatus>();
```

:::

## File

Requires the value to be an uploaded file.

::: code-group

```csharp [Attribute.cs]
[File]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Avatar)
    .File();
```

:::

## Uploaded

Requires the uploaded file to be present and non-empty.

::: code-group

```csharp [Attribute.cs]
[Uploaded]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Avatar)
    .Uploaded();
```

:::

## Extensions

Allows only files with one of the provided file extensions.

::: code-group

```csharp [Attribute.cs]
[Extensions(params string[] extensions)]

[Extensions("jpg", "png", "webp")]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Avatar)
    .Extensions("jpg", "png", "webp");
```

:::

## Mimes

Allows only files matching the provided MIME extension aliases.

::: code-group

```csharp [Attribute.cs]
[Mimes(params string[] mimes)]

[Mimes("jpg", "jpeg", "png", "webp")]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Avatar)
    .Mimes("jpg", "jpeg", "png", "webp");
```

:::

## MimeTypes

Allows only files with one of the provided MIME types.

::: code-group

```csharp [Attribute.cs]
[MimeTypes(params string[] mimeTypes)]

[MimeTypes("image/jpeg", "image/png", "image/webp")]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Avatar)
    .MimeTypes("image/jpeg", "image/png", "image/webp");
```

:::

## Image

Requires the uploaded file to be an image, decided by the bytes it opens with rather than by its name or content type. PNG, GIF, JPEG, WebP, BMP, TIFF, AVIF, HEIC, and ICO are recognized; anything else is rejected, so a file named `photo.png` holding arbitrary content does not pass.

::: code-group

```csharp [Attribute.cs]
[Image]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Avatar)
    .Image();
```

:::

## Dimensions

Requires the uploaded image to match the exact width and height.

::: code-group

```csharp [Attribute.cs]
[Dimensions(int width, int height)]

[Dimensions(512, 512)]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Avatar)
    .Dimensions(512, 512);
```

:::

## MinDimensions

Requires the uploaded image to be at least the provided width and height.

::: code-group

```csharp [Attribute.cs]
[MinDimensions(int width, int height)]

[MinDimensions(512, 512)]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Avatar)
    .MinDimensions(512, 512);
```

:::

## MaxDimensions

Requires the uploaded image to be no larger than the provided width and height.

::: code-group

```csharp [Attribute.cs]
[MaxDimensions(int width, int height)]

[MaxDimensions(1024, 1024)]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Avatar)
    .MaxDimensions(1024, 1024);
```

:::
