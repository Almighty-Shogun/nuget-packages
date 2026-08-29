using System.Reflection;
using Microsoft.Extensions.Logging;
using System.Runtime.ExceptionServices;

namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// The base every console command inherits. It reads the class attributes once per instance and turns the tokens typed at
/// the prompt into the arguments of the single public <c>ExecuteAsync</c> the subclass declares.
/// </summary>
///
/// <remarks>
/// The handler method is found by name rather than by an abstract member, so a command declares whatever parameters it
/// wants and the binder matches them positionally. That is the whole reason this class exists.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
public abstract class ConsoleCommandBase : IConsoleCommand, IInternalConsoleCommand
{
    /// <summary>
    /// The handler resolved once in the constructor, so the reflection cost is paid when the command is built rather than
    /// on every invocation.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private readonly MethodInfo _handlerMethod;

    /// <summary>
    /// The handler's parameters in declaration order, which is also the order arguments are matched in.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private readonly ParameterInfo[] _parameters;

    /// <summary>
    /// The class attribute, kept for the argument-count rule it carries rather than for the name and description, which
    /// are copied onto properties.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private readonly ConsoleCommandAttribute _attribute;

    /// <summary>
    /// Gets the name from the class attribute, available to a subclass that wants to mention itself in its own output.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    protected string Name { get; }

    /// <summary>
    /// Gets the description from the class attribute, or <c>null</c> when the command declares none.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    protected string? Description { get; }

    /// <summary>
    /// Gets the aliases from the class attribute, or an empty list when the command declares none.
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
    /// Validates the subclass and caches its handler. Validation happens here rather than at registration, so a malformed
    /// command fails when the handler is resolved and names the offending class instead of quietly never appearing.
    /// </summary>
    ///
    /// <exception cref="InvalidOperationException">
    /// Thrown when the class carries no <see cref="ConsoleCommandAttribute"/>, declares anything other than exactly one
    /// public <c>ExecuteAsync</c>, or declares one returning anything other than <see cref="Task"/> or
    /// <see cref="ValueTask"/>.
    /// </exception>
    ///
    /// <remarks>
    /// Deliberately takes nothing. A command reports bad input through the dispatcher's logger rather than one of its own,
    /// so a command that needs no services of its own declares no constructor at all.
    /// </remarks>
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
                logger.LogWarning(
                    "Invalid number of parameters on command {Name:c}. Expected {ParametersLength}, got {ArgsLength}",
                    Name,
                    boundParameters.Length,
                    args.Length
                );

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
                _ => Task.CompletedTask
            });
        }
        catch (TargetInvocationException exception) when (exception.InnerException is not null)
        {
            ExceptionDispatchInfo.Capture(exception.InnerException).Throw();
        }
    }
}
