using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Owns the console input loop and forwards complete input lines to the command dispatcher.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
internal sealed class ConsoleCommandHandler : IConsoleCommandHandler
{
    /// <summary>
    /// Dispatches non-empty input lines to registered commands.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly ConsoleCommandDispatcher _dispatcher;

    /// <summary>
    /// The logger used for input-loop and lifecycle failures.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private readonly ILogger<ConsoleCommandHandler> _logger;

    /// <summary>
    /// Synchronizes access to the handler lifecycle state.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private readonly Lock _lifecycleGate = new();

    /// <summary>
    /// The active console reader thread, or <c>null</c> when no reader remains.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private Thread? _readerThread;

    /// <summary>
    /// Cancels the active command loop and indicates whether one is running.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private CancellationTokenSource? _stopSource;


    /// <summary>
    /// Creates a console command handler and its command dispatcher.
    /// </summary>
    ///
    /// <param name="logger">The logger used by the handler and dispatcher.</param>
    /// <param name="scopeFactory">The factory used to create command invocation scopes.</param>
    /// <param name="descriptors">The registered console command descriptors.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public ConsoleCommandHandler(ILogger<ConsoleCommandHandler> logger,
        IServiceScopeFactory scopeFactory,
        IEnumerable<ConsoleCommandDescriptor> descriptors)
    {
        _dispatcher = new ConsoleCommandDispatcher(logger, scopeFactory, descriptors, EmitCommandFailed);
        _logger = logger;
    }


    /// <inheritdoc />
    public event EventHandler<ConsoleCommandErrorEvent>? CommandFailed;

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        CancellationTokenSource stopSource;

        lock (_lifecycleGate)
        {
            if (_stopSource is not null)
            {
                _logger.LogError("Cannot start the console command handler because it is already running.");

                return;
            }

            if (_readerThread is not null)
            {
                _logger.LogError("Cannot start the console command handler because the previous console reader is still stopping.");

                return;
            }

            stopSource = _stopSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        }

        bool? previousTreatControlCAsInput = null;
        try
        {
            try
            {
                previousTreatControlCAsInput = Console.TreatControlCAsInput;
                Console.TreatControlCAsInput = false;
            }
            catch (IOException) { }

            Channel<string> lines = Channel.CreateUnbounded<string>(
                new UnboundedChannelOptions
                {
                    SingleReader = true,
                    SingleWriter = true
                });

            var requests = new SemaphoreSlim(0, 1);
            var readerStop = new CancellationTokenSource();

            var reader = new Thread(() =>
            {
                try
                {
                    ReadLines(lines.Writer, requests, readerStop.Token);
                }
                finally
                {
                    lock (_lifecycleGate)
                    {
                        if (ReferenceEquals(_readerThread, Thread.CurrentThread))
                            _readerThread = null;

                        readerStop.Dispose();
                        requests.Dispose();
                    }
                }
            })
            {
                IsBackground = true,
                Name = "Console command reader"
            };

            var readerStarted = false;

            lock (_lifecycleGate)
            {
                _readerThread = reader;
            }

            try
            {
                reader.Start();
                readerStarted = true;

                while (!stopSource.IsCancellationRequested)
                {
                    string input;

                    requests.Release();

                    try
                    {
                        input = await lines.Reader.ReadAsync(stopSource.Token);
                    }
                    catch (OperationCanceledException) when (stopSource.IsCancellationRequested)
                    {
                        break;
                    }
                    catch (ChannelClosedException exception) when (exception.InnerException is null)
                    {
                        break;
                    }

                    if (string.IsNullOrWhiteSpace(input)) continue;

                    Utils.ConsoleUtils.RemoveLastLine();

                    await _dispatcher.DispatchAsync(input, stopSource.Token);
                }
            }
            catch (OperationCanceledException) when (stopSource.IsCancellationRequested) { }
            catch (Exception exception)
            {
                _logger.LogError(exception, "The console command handler stopped unexpectedly.");
            }
            finally
            {
                lines.Writer.TryComplete();

                lock (_lifecycleGate)
                {
                    if (!readerStarted)
                    {
                        if (ReferenceEquals(_readerThread, reader))
                            _readerThread = null;

                        readerStop.Dispose();
                        requests.Dispose();
                    }
                    else if (ReferenceEquals(_readerThread, reader))
                    {
                        readerStop.Cancel();
                    }
                }
            }
        }
        finally
        {
            if (previousTreatControlCAsInput is { } previous)
            {
                try
                {
                    Console.TreatControlCAsInput = previous;
                }
                catch (IOException) { }
            }

            lock (_lifecycleGate)
            {
                _stopSource?.Dispose();
                _stopSource = null;
            }
        }
    }

    /// <summary>
    /// Raises <see cref="CommandFailed"/> for each subscriber and isolates subscriber failures.
    /// </summary>
    ///
    /// <param name="commandName">The name of the command that failed.</param>
    /// <param name="exception">The command failure.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private void EmitCommandFailed(
        string commandName,
        Exception exception)
    {
        EventHandler<ConsoleCommandErrorEvent>? handlers = CommandFailed;

        if (handlers is null)
            return;

        var args = new ConsoleCommandErrorEvent(commandName, exception);

        foreach (Delegate @delegate
                 in handlers.GetInvocationList())
        {
            var handler = (EventHandler<ConsoleCommandErrorEvent>)@delegate;
            try
            {
                handler(this, args);
            }
            catch (Exception subscriberException)
            {
                _logger.LogError(
                    subscriberException,
                    "A console command failure handler threw an exception");
            }
        }
    }


    /// <summary>
    /// Reads one console line for each request and publishes it to the input channel.
    /// </summary>
    ///
    /// <param name="writer">The channel receiving input lines.</param>
    /// <param name="requests">Signals when another line should be read.</param>
    /// <param name="cancellationToken">Stops the reader while it is waiting for another request.</param>
    ///
    /// <remarks>
    /// A blocking console read cannot be cancelled once it has started, so the reader may outlive the command loop until that
    /// read completes.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static void ReadLines(ChannelWriter<string> writer, SemaphoreSlim requests, CancellationToken cancellationToken)
    {
        try
        {
            while (true)
            {
                requests.Wait(cancellationToken);

                if (Console.In.ReadLine() is not { } line)
                {
                    writer.TryComplete();

                    return;
                }

                if (!writer.TryWrite(line)) return;
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            writer.TryComplete();
        }
        catch (Exception exception)
        {
            writer.TryComplete(exception);
        }
    }

    /// <inheritdoc />
    public void Stop()
    {
        lock (_lifecycleGate)
        {
            if (_stopSource is null)
            {
                _logger.LogError("Cannot stop the console command handler because it is not running.");

                return;
            }

            try
            {
                _stopSource.Cancel();
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Failed to stop the console command handler.");
            }
        }
    }
}
