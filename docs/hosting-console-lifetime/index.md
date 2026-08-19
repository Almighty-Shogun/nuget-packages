# Hosting Console Lifetime

Adjusts how a .NET generic host starts and stops. The package replaces the default console lifetime so that `Ctrl+C` cannot kill a long-running service by accident, while a `SIGTERM` from an orchestrator still shuts the host down cleanly.

Use it in worker services, background processors, and console applications that run unattended, especially anything deployed to Docker, Kubernetes, or systemd, where the supervisor stops the process with `SIGTERM`.

## Categories

- [Extensions](./extensions/use-custom-console-lifetime) &mdash; host lifetime replacement and host option configuration.

## Quick Example

```csharp
using Microsoft.Extensions.Hosting;
using AlmightyShogun.Hosting.ConsoleLifetime;
using Microsoft.Extensions.DependencyInjection;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.UseCustomConsoleLifetime();
builder.ConfigureHostOptions(
    TimeSpan.FromSeconds(30),
    BackgroundServiceExceptionBehavior.StopHost
);

builder.Services.AddHostedService<ImportWorker>();

await builder.Build().RunAsync();
```
