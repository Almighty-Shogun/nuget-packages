using Microsoft.Extensions.Hosting;
using System.Runtime.InteropServices;

namespace AlmightyShogun.Hosting.ConsoleLifetime;

/// <summary>
/// Provides a custom console lifetime for hosted applications.
/// </summary>
///
/// <param name="applicationLifetime"> The application lifetime.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>2.0.0</since>
internal sealed class CustomConsoleLifetime(IHostApplicationLifetime applicationLifetime) : IHostLifetime, IDisposable
{
    /// <summary>
    /// Indicates whether the application is running in an IDE.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.0.0</since>
    private readonly bool _runningInIde = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_IDE"));

    /// <summary>
    /// Stores the <c>SIGTERM</c> registration.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private PosixSignalRegistration? _sigTermRegistration;

    /// <inheritdoc />
    ///
    /// <exception cref="PlatformNotSupportedException">
    /// Signal handling is not supported by the platform.
    /// </exception>
    /// <exception cref="IOException">
    /// The signal handler could not be registered.
    /// </exception>
    public Task WaitForStartAsync(CancellationToken cancellationToken)
    {
        Console.CancelKeyPress += OnCancelKeyPress;

        if (!OperatingSystem.IsWindows())
            _sigTermRegistration = PosixSignalRegistration.Create(
                PosixSignal.SIGTERM,
                context =>
                {
                    context.Cancel = true;

                    applicationLifetime.StopApplication();
                });

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc />
    public void Dispose()
    {
        Console.CancelKeyPress -= OnCancelKeyPress;
        _sigTermRegistration?.Dispose();
    }

    /// <summary>
    /// Handles console cancellation.
    /// </summary>
    ///
    /// <param name="sender">The event sender.</param>
    /// <param name="eventArgs">The console cancellation event arguments.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.0.0</since>
    private void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs eventArgs) => eventArgs.Cancel = !_runningInIde;
}
