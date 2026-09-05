using Microsoft.Extensions.Hosting;
using System.Runtime.InteropServices;

namespace AlmightyShogun.Hosting.ConsoleLifetime;

/// <summary>
/// Keeps a console application alive when <c>Ctrl+C</c> is pressed, unless <c>DOTNET_RUNNING_IN_IDE</c> is set. Off Windows
/// it also registers a <c>SIGTERM</c> handler
/// that shuts the host down in an orderly way; on Windows none is registered. Registered through
/// <see cref="ConsoleLifetimeExtensions"/>; it is never constructed by consumer code.
/// </summary>
///
/// <param name="applicationLifetime">
/// The lifetime asked to begin shutdown. The <c>SIGTERM</c> handler is its only call site, so on Windows, where that
/// handler is never registered, it is never called. Going through it lets hosted services run their stop path, rather than
/// the process ending where it stands.
/// </param>
///
/// <remarks>
/// The host resolves a single <see cref="IHostLifetime"/> and takes the last registration. Replacing the default rather
/// than adding alongside it keeps the count at one, but the outcome is still order-dependent: <c>Replace</c> removes the
/// first matching registration and appends the new one, so a later plain <c>Add</c> for <see cref="IHostLifetime"/> wins.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>2.0.0</since>
internal sealed class CustomConsoleLifetime(IHostApplicationLifetime applicationLifetime) : IHostLifetime, IDisposable
{
    /// <summary>
    /// Holds whether <c>DOTNET_RUNNING_IN_IDE</c> was set to a non-empty value, read once when the instance is constructed.
    /// When it is, <c>Ctrl+C</c> keeps working, so a debug session can still be stopped the usual way. Nothing checks that an
    /// IDE actually set it; whoever launches the process decides.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.0.0</since>
    private readonly bool _runningInIde = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_IDE"));

    /// <summary>
    /// Holds the <c>SIGTERM</c> handler so it can be released with the lifetime. Stays null on Windows, where no
    /// registration is made. Since <c>Ctrl+C</c> is suppressed there too, nothing in this type asks the host to stop on
    /// Windows at all; only a call to <see cref="IHostApplicationLifetime.StopApplication"/> or to <c>IHost.StopAsync</c>
    /// does. Setting <c>DOTNET_RUNNING_IN_IDE</c> stops suppressing the key press, which terminates the process rather than
    /// shutting the host down in an orderly way.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private PosixSignalRegistration? _sigTermRegistration;

    /// <inheritdoc />
    ///
    /// <exception cref="PlatformNotSupportedException">
    /// <c>SIGTERM</c> is not supported by the platform, as documented on <see cref="PosixSignalRegistration.Create"/>.
    /// Nothing here catches it, so it escapes into <see cref="IHost.StartAsync"/>.
    /// </exception>
    /// <exception cref="IOException">
    /// Setting up the signal handling or installing the handler failed, as documented on
    /// <see cref="PosixSignalRegistration.Create"/>. Nothing here catches it either.
    /// </exception>
    ///
    /// <remarks>
    /// The <c>SIGTERM</c> handler cancels the default handling of the signal before requesting shutdown, so the runtime does
    /// not terminate the process while hosted services are still stopping. <see cref="HostOptions.ShutdownTimeout"/>, which
    /// <see cref="ConsoleLifetimeExtensions"/> sets when its host options helper is called, still caps how long the host
    /// waits for them.
    ///
    /// The registration is made only off Windows, and <paramref name="cancellationToken"/> is not observed.
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
    /// Nothing to wait for. Shutdown is driven by the <c>SIGTERM</c> handler where one was registered, by anything that calls
    /// <see cref="IHostApplicationLifetime.StopApplication"/>, and by a direct <c>IHost.StopAsync</c>, so by the time this
    /// runs the decision has already been made. <paramref name="cancellationToken"/> is not observed.
    /// </remarks>
    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc />
    ///
    /// <remarks>
    /// <see cref="Console.CancelKeyPress"/> is static and the handler holds no reference to the host, so a leaked subscription
    /// would keep swallowing key presses for the rest of the process. That bites hardest where a host is built and disposed
    /// more than once, such as in tests.
    /// </remarks>
    public void Dispose()
    {
        Console.CancelKeyPress -= OnCancelKeyPress;
        _sigTermRegistration?.Dispose();
    }

    /// <summary>
    /// Swallows <c>Ctrl+C</c> and <c>Ctrl+Break</c>, which raise the same event, so an operator cannot stop a long-running
    /// process by accident, except when <c>DOTNET_RUNNING_IN_IDE</c> is set.
    /// </summary>
    ///
    /// <param name="sender">
    /// Whatever <see cref="Console.CancelKeyPress"/> supplies. Unused; the decision depends only on the environment.
    /// </param>
    /// <param name="eventArgs">
    /// Carries the cancel flag. Setting it keeps the process running, so it is set for either key combination unless
    /// <c>DOTNET_RUNNING_IN_IDE</c> is set.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>2.0.0</since>
    private void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs eventArgs) => eventArgs.Cancel = !_runningInIde;
}
