# MessageResolver

Resolves localized messages from the JSON message files described on the [localization](../localization) page. Application code depends on `IMessageResolver`. Resolution never fails: a key the negotiated language does not define is returned as itself, so a missing translation degrades to a readable identifier instead of an exception or a blank body.

## Resolve

Resolves a message key, optionally formatting it with parameters. Parameters are substituted with `string.Format`, so the template uses `{0}`, `{1}`, and so on. They are formatted in the culture of the language the message resolved in, so a number in a Dutch message reads `1234,56` rather than taking the server's culture.

The language is negotiated once through `ResolveLanguage` and every key comes from that one language, so a key it does not define comes back as itself rather than being looked up in the next accepted language.

A template whose placeholders do not match the supplied parameters returns the raw template rather than throwing, so a bad message file cannot fail a request.

::: code-group

```csharp [OrdersController.cs]
using Microsoft.AspNetCore.Mvc;
using AlmightyShogun.AspNet.Localization;

[ApiController]
[Route("orders")]
public sealed class OrdersController(
    IMessageResolver messageResolver
) : ControllerBase
{
    [HttpGet("{id:int}")]
    public IActionResult Get(int id)
    {
        Order? order = FindOrder(id);

        return order is null
            ? NotFound(messageResolver.Resolve("orders.not-found", [id]))
            : Ok(order);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
        => BadRequest(messageResolver.Resolve("orders.read-only"));
}
```

```json [messages/en/orders.json]
{
    "not-found": "Order {0} does not exist",
    "read-only": "Orders cannot be deleted"
}
```

:::

### Type signature

```csharp
public string Resolve(string key);

public string Resolve(string key, IReadOnlyList<object?> parameters);
```

## ResolveLanguage

Returns the language messages are currently being served in: the first candidate in the negotiation chain whose directory holds any messages, or `DefaultLanguage` when none does. This is what `Resolve` reads from and what the middleware writes to the response `Content-Language` header.

Useful when a response needs to state its own language, for example an email body rendered from the same message files.

```csharp
using AlmightyShogun.AspNet.Localization;

string language = messageResolver.ResolveLanguage();
```

### Type signature

```csharp
public string ResolveLanguage();
```
