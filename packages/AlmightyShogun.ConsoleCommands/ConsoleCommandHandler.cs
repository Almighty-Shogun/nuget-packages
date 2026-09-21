using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Reads the console a line at a time and dispatches each line to the command registered under its first token. Commands
/// are resolved per invocation from a fresh scope, so one may depend on scoped application services.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
internal sealed class ConsoleCommandHandler : IConsoleCommandHandler
{
    private readonly ConsoleCommandDispatcher _dispatcher;
    private readonly ILogger<ConsoleCommandHandler> _logger;

    /// <summary>
    /// Guards the lifecycle source, so <see cref="Stop"/> cannot observe it between the null check and the cancel.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private readonly Lock _lifecycleGate = new();

    private Thread? _readerThread;

    /// <summary>
    /// The source canceled to end the running loop, and the flag for whether one is running at all. It is set before the
    /// setup the loop needs, the reader thread included, and cleared in the loop's <c>finally</c>, so a failure in that
    /// setup leaves it set with no loop running, after which every further <see cref="StartAsync"/> logs that one is
    /// already running and returns.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private CancellationTokenSource? _stopSource;
    
    

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
    /// Reads standard input on a thread of its own, one line for each one the loop asks for, so that a read already under way
    /// is abandoned rather than waited out when the handler stops and nothing is taken off the input between two requests.
    /// </summary>
    ///
    /// <param name="writer">
    /// The channel end the lines are published to. Reaching the end of the input stream completes it, which is how the loop
    /// learns there will be no further line.
    /// </param>
    /// <param name="requests">
    /// Released once by the loop for every line it wants. Nothing is read until one has been taken, which is what leaves
    /// standard input to a command for as long as one is running.
    /// </param>
    /// <param name="cancellationToken">
    /// Signaled by the loop on its way out, ending the wait for the next request. That wait is the only point this can be
    /// stopped at, since a read already under way has to finish first.
    /// </param>
    ///
    /// <remarks>
    /// This runs on a background thread, because a read parked in <see cref="TextReader.ReadLine"/> cannot be interrupted
    /// and a foreground thread sitting in one would hold the process open. A stop that interrupts a read leaves the thread
    /// to outlive <see cref="StartAsync"/> by however long the next line or the end of the stream takes to arrive: the loop
    /// completes the channel on its way out, so the line that finally arrives is refused by the channel, dropped, and the
    /// thread ends.
    ///
    /// A read that fails while no stop has been asked for completes the channel with the exception rather than as an
    /// ordinary end of input, so it reaches the loop and is reported as an unexpected stop. It arrives wrapped in a
    /// <see cref="ChannelClosedException"/>, except for an <see cref="OperationCanceledException"/>, which completes the
    /// channel as a cancellation and reaches the loop as one. That is why the loop ends quietly on a cancellation only once
    /// a stop has actually been asked for. A cancellation raised once the stop is under way, by the wait for the next
    /// request or by the read itself, completes the channel as an ordinary end of input instead.
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
