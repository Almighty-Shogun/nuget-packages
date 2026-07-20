# Format Rules

Format rules validate common string formats such as email addresses, URLs, identifiers, JSON, IP addresses, and MAC addresses. Use them after presence rules when a missing or empty value should produce a required-style error first.

## Email

Requires the value to be a valid email address.

::: code-group

```csharp [Attribute.cs]
[Email]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Email)
    .Email();
```

:::

## HexColor

Requires the value to be a hexadecimal color value.

::: code-group

```csharp [Attribute.cs]
[HexColor]
```

```csharp [FluentRule.cs]
RuleFor(x => x.BrandColor)
    .HexColor();
```

:::

## Uuid

Requires the value to be a valid UUID value.

::: code-group

```csharp [Attribute.cs]
[Uuid]
```

```csharp [FluentRule.cs]
RuleFor(x => x.ExternalId)
    .Uuid();
```

:::

## Ulid

Requires the value to be a valid ULID value.

::: code-group

```csharp [Attribute.cs]
[Ulid]
```

```csharp [FluentRule.cs]
RuleFor(x => x.PublicId)
    .Ulid();
```

:::

## Ip

Requires the value to be a valid IPv4 or IPv6 address.

::: code-group

```csharp [Attribute.cs]
[Ip]
```

```csharp [FluentRule.cs]
RuleFor(x => x.ClientIp)
    .Ip();
```

:::

## Ipv4

Requires the value to be a valid IPv4 address.

::: code-group

```csharp [Attribute.cs]
[Ipv4]
```

```csharp [FluentRule.cs]
RuleFor(x => x.ClientIp)
    .Ipv4();
```

:::

## Ipv6

Requires the value to be a valid IPv6 address.

::: code-group

```csharp [Attribute.cs]
[Ipv6]
```

```csharp [FluentRule.cs]
RuleFor(x => x.ClientIp)
    .Ipv6();
```

:::

## MacAddress

Requires the value to be a valid MAC address.

::: code-group

```csharp [Attribute.cs]
[MacAddress]
```

```csharp [FluentRule.cs]
RuleFor(x => x.DeviceAddress)
    .MacAddress();
```

:::

## Json

Requires the value to be valid JSON text.

::: code-group

```csharp [Attribute.cs]
[Json]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Metadata)
    .Json();
```

:::

## Url

Requires the value to be an HTTP or HTTPS URL.

::: code-group

```csharp [Attribute.cs]
[Url]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Website)
    .Url();
```

:::

## Timezone

Requires the value to be a valid time zone identifier.

::: code-group

```csharp [Attribute.cs]
[Timezone]
```

```csharp [FluentRule.cs]
RuleFor(x => x.Timezone)
    .Timezone();
```

:::
