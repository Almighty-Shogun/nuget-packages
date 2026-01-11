using Microsoft.Extensions.Hosting;
using System.Runtime.InteropServices;

namespace AlmightyShogun.Hosting.Utils;

internal class CustomConsoleLifetime : IHostLifetime, IDisposable
{
    private readonly bool _runningInIde = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_IDE"));

    /// <inheritdoc />
    public Task WaitForStartAsync(CancellationToken cancellationToken)
    {
        Console.CancelKeyPress += OnCancelKeyPress;
        
        if (!OperatingSystem.IsWindows())
        {
            PosixSignalRegistration.Create(PosixSignal.SIGTERM, ctx => ctx.Cancel = true);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc />
    public void Dispose()
    {
        Console.CancelKeyPress -= OnCancelKeyPress;
        
        GC.SuppressFinalize(this);
    }
    
    /// <summary>
    /// Handles the Ctrl+C (or SIGINT) event in the console.
    /// Allows the host to shut down only if the application is running inside an IDE.
    /// </summary>
    /// 
    /// <param name="sender">The source of the event (typically the Console).</param>
    /// <param name="eventArgs">The <see cref="ConsoleCancelEventArgs"/> containing event data, including the Cancel flag.</param>
    ///
    /// <authors>Almighty-Shogun</authors>
    /// <since>2.0.0</since>
    private void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs eventArgs)
    {
        eventArgs.Cancel = !_runningInIde;
    }
}
