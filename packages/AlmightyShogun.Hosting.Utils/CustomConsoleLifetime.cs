using Microsoft.Extensions.Hosting;
using System.Runtime.InteropServices;

namespace AlmightyShogun.Hosting.Utils;

/// <summary>
/// Provides a host lifetime that suppresses Ctrl+C shutdown outside IDE-hosted runs.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.0.0</since>
internal class CustomConsoleLifetime : IHostLifetime, IDisposable
{
    /// <summary>
    /// Gets whether the current process was started from an IDE run configuration.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.0.0</since>
    private readonly bool _runningInIde = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_IDE"));

    /// <summary>
    /// Stores the POSIX SIGTERM registration so it can be disposed with the lifetime.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private PosixSignalRegistration? _sigTermRegistration;

    /// <inheritdoc />
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.0.0</since>
    public Task WaitForStartAsync(CancellationToken cancellationToken)
    {
        Console.CancelKeyPress += OnCancelKeyPress;

        if (!OperatingSystem.IsWindows())
        {
            _sigTermRegistration = PosixSignalRegistration.Create(PosixSignal.SIGTERM, ctx => ctx.Cancel = true);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.0.0</since>
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc />
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.0.0</since>
    public void Dispose()
    {
        Console.CancelKeyPress -= OnCancelKeyPress;
        _sigTermRegistration?.Dispose();

        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Handles Ctrl+C by allowing shutdown only when the application is running inside an IDE.
    /// </summary>
    ///
    /// <param name="sender">The event source.</param>
    /// <param name="eventArgs">The console cancel event arguments whose cancel flag controls whether shutdown continues.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.0.0</since>
    private void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs eventArgs)
    {
        eventArgs.Cancel = !_runningInIde;
    }
}
