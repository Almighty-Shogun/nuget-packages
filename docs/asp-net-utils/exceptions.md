# Exceptions

The package defines no exceptions of its own. It defines `IAppException`, the contract every exception across this repository implements to become a standardized error response, and the one application code implements to fail a request deliberately. An exception implementing it is recognized only once [`AddExceptionHandling`](./extensions/add-exception-handling) and [`UseHttpErrorResponses`](./extensions/use-http-error-responses) are both in place.

## IAppException

Marks an exception that carries everything needed to produce a standardized error response. [`AppExceptionHandler`](./handlers/app-exception-handler) recognizes it before any other handler, so the response uses the status code and error code the exception names rather than falling through to a `500`.

::: code-group

```csharp [InvalidCredentialsException.cs]
using Microsoft.AspNetCore.Http;
using AlmightyShogun.AspNet.Utils;

public sealed class InvalidCredentialsException : Exception, IAppException
{
    public int StatusCode => StatusCodes.Status401Unauthorized;
    public string Code => "invalid_credentials";
    public string MessageKey => "auth.failed";
    public object?[] MessageParameters => [];
}
```

```csharp [AccountService.cs]
public sealed class AccountService
{
    public async Task<Account> AuthenticateAsync(string email, string password)
    {
        Account? account = await FindAsync(email);

        if (account is null || !Verify(account, password))
        {
            throw new InvalidCredentialsException();
        }

        return account;
    }
}
```

:::

### Members

`StatusCode` is the HTTP status to return. It also decides the log level: `500` and above are logged with the exception, anything lower without it.

`Code` is the stable machine-readable identifier clients branch on. Treat it as public API: renaming it breaks consumers even though nothing fails to compile.

`MessageKey` is resolved through [`MessageResolver`](./services/message-resolver) to produce `errorDescription`, so the description follows the request's language. A key no message file defines is returned verbatim.

`MessageParameters` are the values formatted into the resolved template by position, as `{0}` and onwards. Return an empty array when the message takes none.

### Type signature

```csharp
public interface IAppException
{
    int StatusCode { get; }
    string Code { get; }
    string MessageKey { get; }
    object?[] MessageParameters { get; }
}
```
