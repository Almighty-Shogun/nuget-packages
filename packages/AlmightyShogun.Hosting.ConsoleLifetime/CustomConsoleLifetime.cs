using Microsoft.Extensions.Hosting;
using System.Runtime.InteropServices;

namespace AlmightyShogun.Hosting.ConsoleLifetime;

/// <summary>
/// Keeps a console application alive when <c>Ctrl+C</c> is pressed, while still shutting down in an orderly way on
/// <c>SIGTERM</c>. Registered through <see cref="PackageRegistry"/>; it is never constructed by consumer code.
/// </summary>
///
/// <param name="applicationLifetime">
/// The lifetime asked to begin shutdown when a termination signal arrives. Going through it lets hosted services run their
/// stop path, rather than the process ending where it stands.
/// </param>
///
/// <remarks>
/// The host resolves a single <see cref="IHostLifetime"/>, so this type only takes effect when it replaces the default
/// console lifetime rather than being added alongside it.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>2.0.0</since>
internal sealed class CustomConsoleLifetime(IHostApplicationLifetime applicationLifetime) : IHostLifetime, IDisposable
{
    /// <summary>
    /// Tracks whether the process was started from an IDE run configuration, read once from <c>DOTNET_RUNNING_IN_IDE</c>. When
    /// it is set, <c>Ctrl+C</c> keeps working, so a debug session can still be stopped the usual way.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.0.0</since>
    private readonly bool _runningInIde = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_IDE"));

    /// <summary>
    /// Holds the <c>SIGTERM</c> handler so it can be released with the lifetime. Stays null on Windows, where the signal does
    /// not exist and no registration is made.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private PosixSignalRegistration? _sigTermRegistration;

    /// <inheritdoc />
    ///
    /// <remarks>
    /// The <c>SIGTERM</c> handler marks the signal as handled before requesting shutdown, so the runtime does not terminate
    /// the process while hosted services are still stopping. Without that, a container stop would cut shutdown short.
    /// </remarks>
    public Task WaitForStartAsync(CancellationToken cancellationToken)
    {
        Console.CancelKeyPress += OnCancelKeyPress;

        if (!OperatingSystem.IsWindows())
            _sigTermRegistration = PosixSignalRegistration.Create(PosixSignal.SIGTERM, context =>
            {
                context.Cancel = true;

                applicationLifetime.StopApplication();
            });

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    ///
    /// <remarks>
    /// Nothing to wait for. Shutdown is driven by the signal handler and by anything that calls
    /// <see cref="IHostApplicationLifetime.StopApplication"/>, so by the time this runs the decision has already been made.
    /// </remarks>
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc />
    ///
    /// <remarks>
    /// Detaching the handler matters for a host that is built and disposed more than once in a process, such as in tests,
    /// where a leaked subscription would keep answering key presses on behalf of a host that is gone.
    /// </remarks>
    public void Dispose()
    {
        Console.CancelKeyPress -= OnCancelKeyPress;
        _sigTermRegistration?.Dispose();
    }

    /// <summary>
    /// Swallows <c>Ctrl+C</c> so an operator cannot stop a long-running process by accident, except when running in an IDE.
    /// </summary>
    ///
    /// <param name="sender">The console raising the key press. Unused; the decision depends only on the environment.</param>
    /// <param name="eventArgs">
    /// Carries the cancel flag. Setting it keeps the process running, so it is set in every case except an IDE run.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.0.0</since>
    private void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs eventArgs) => eventArgs.Cancel = !_runningInIde;
}
