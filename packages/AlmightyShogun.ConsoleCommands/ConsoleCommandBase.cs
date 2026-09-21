using System.Reflection;
using Microsoft.Extensions.Logging;
using System.Runtime.ExceptionServices;

namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Base class for console commands.
///
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
public abstract class ConsoleCommandBase : IConsoleCommand, IInternalConsoleCommand
{
    private readonly MethodInfo _handlerMethod;
    
    private readonly ParameterInfo[] _parameters;
    
    private readonly ConsoleCommandAttribute _attribute;

    /// <summary>
    /// Gets the primary command name.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    protected string Name { get; }

    /// <summary>
    /// Gets the command description, if any.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    protected string? Description { get; }

    /// <summary>
    /// Gets the command aliases.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    protected IReadOnlyList<string> Aliases { get; }

    /// <inheritdoc />
    string IConsoleCommand.Name => Name;

    /// <inheritdoc />
    string? IConsoleCommand.Description => Description;

    /// <inheritdoc />
    IReadOnlyList<string> IConsoleCommand.Aliases => Aliases;

    /// <summary>
    /// Initializes and validates a console command.
    /// </summary>
    ///
    /// <exception cref="InvalidOperationException">
    /// Thrown when the derived command does not satisfy the required command shape.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    protected ConsoleCommandBase()
    {
        if (!CommandMetadata.TryDescribe(GetType(), out ConsoleCommandAttribute attribute, out MethodInfo handlerMethod, out string? error))
            throw new InvalidOperationException(error);

        _attribute = attribute;
        _handlerMethod = handlerMethod;
        _parameters = handlerMethod.GetParameters();

        Name = attribute.Name;
        Description = attribute.Description;
        Aliases = GetType().GetCustomAttribute<AliasAttribute>()?.Aliases ?? [];
    }

    /// <inheritdoc />
    async Task IInternalConsoleCommand.InternallyExecuteCommandAsync(string[] args, ILogger logger, CancellationToken cancellationToken)
    {
        ParameterInfo[] boundParameters = _parameters;
        bool takesCancellationToken = _parameters.Length > 0 && _parameters[^1].ParameterType == typeof(CancellationToken);

        if (takesCancellationToken)
            boundParameters = _parameters[..^1];

        if (!CommandArgumentBinder.IsArgumentCountValid(boundParameters, args.Length, _attribute.IgnoreExtraArgs))
        {
            if (logger.IsEnabled(LogLevel.Warning))
            {
                if (CommandArgumentBinder.HasVariadicTail(boundParameters))
                    logger.LogWarning(
                        "Invalid number of parameters on command {Name:c}. Expected at least {ParametersLength}, got {ArgsLength}",
                        Name,
                        CommandArgumentBinder.GetMinimumArgumentCount(boundParameters),
                        args.Length
                    );
                else
                    logger.LogWarning(
                        "Invalid number of parameters on command {Name:c}. Expected {ParametersLength}, got {ArgsLength}",
                        Name,
                        boundParameters.Length,
                        args.Length
                    );
            }

            return;
        }

        if (!CommandArgumentBinder.TryBind(boundParameters, args, logger, out object?[] values)) return;

        object?[] invocationValues = takesCancellationToken ? [.. values, cancellationToken] : values;

        try
        {
            await (_handlerMethod.Invoke(this, invocationValues) switch
            {
                Task task => task,
                ValueTask valueTask => valueTask.AsTask(),
                null => throw new InvalidOperationException(
                    $"{_handlerMethod.DeclaringType?.Name}.{_handlerMethod.Name} returned null."),
                _ => throw new InvalidOperationException(
                    $"{_handlerMethod.DeclaringType?.Name}.{_handlerMethod.Name} returned an unsupported result.")
                
            });
        }
        catch (TargetInvocationException exception) when (exception.InnerException is not null)
        {
            ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
        }
    }
}
