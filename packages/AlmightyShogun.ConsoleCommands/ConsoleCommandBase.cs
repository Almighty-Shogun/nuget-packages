using System.Reflection;
using Microsoft.Extensions.Logging;

namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Provides the reflection and argument binding pipeline used by class-based console commands.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
public abstract class ConsoleCommandBase : IConsoleCommand, IInternalConsoleCommand
{
    /// <summary>
    /// Stores aliases declared on the command class.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private readonly List<string> _aliases = [];

    /// <summary>
    /// Stores the public <c>ExecuteAsync</c> method invoked when the command runs.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private readonly MethodInfo? _handlerMethod;

    /// <summary>
    /// Stores the reflected parameters of the command handler method.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private readonly ParameterInfo[] _parameters = [];

    /// <summary>
    /// Stores the logger used for argument binding and execution warnings.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private readonly ILogger<ConsoleCommandBase> _logger;

    /// <summary>
    /// Stores the command metadata declared on the command class.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private readonly ConsoleCommandAttribute? _attribute;

    /// <summary>
    /// Gets the command name used by the input dispatcher.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    internal string Name { get; }

    /// <summary>
    /// Gets the optional command description declared on the command class.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    internal string? Description { get; }

    /// <summary>
    /// Gets the aliases declared on the command class.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    internal IReadOnlyList<string> Aliases => _aliases;

    /// <inheritdoc />
    string IConsoleCommand.Name => Name;

    /// <inheritdoc />
    string? IConsoleCommand.Description => Description;

    /// <inheritdoc />
    IReadOnlyList<string> IConsoleCommand.Aliases => Aliases;

    /// <summary>
    /// Creates a console command instance and validates the required class-level metadata and handler method.
    /// </summary>
    ///
    /// <param name="logger">The logger used to report invalid command arguments.</param>
    ///
    /// <exception cref="InvalidOperationException">Thrown when the command class is missing required metadata or does not expose exactly one valid <c>ExecuteAsync</c> method.</exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    protected ConsoleCommandBase(ILogger<ConsoleCommandBase> logger)
    {
        _logger = logger;

        _attribute = GetType().GetCustomAttribute<ConsoleCommandAttribute>()
            ?? throw new InvalidOperationException($"{GetType().Name} must define {nameof(ConsoleCommandAttribute)} on the class.");

        MethodInfo[] handlerMethods = GetType()
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Where(m => string.Equals(m.Name, "ExecuteAsync", StringComparison.Ordinal))
            .ToArray();

        if (handlerMethods.Length != 1)
        {
            throw new InvalidOperationException(
                $"{GetType().Name} must define exactly one public instance method named ExecuteAsync.");
        }

        _handlerMethod = handlerMethods[0];

        if (_handlerMethod.ReturnType != typeof(Task))
        {
            throw new InvalidOperationException($"{GetType().Name}.ExecuteAsync must return {nameof(Task)}.");
        }

        Name = _attribute.Name;
        Description = _attribute.Description;

        var aliasAttribute = GetType().GetCustomAttribute<AliasAttribute>();

        _aliases.AddRange(aliasAttribute?.Aliases ?? []);

        _parameters = _handlerMethod.GetParameters();
    }

    /// <inheritdoc />
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    async Task IInternalConsoleCommand.InternallyExecuteCommandAsync(string[] args)
    {
        ParameterInfo[] realParameters = _parameters.Where(p => !p.HasDefaultValue).ToArray();

        if (args.Length < realParameters.Length || !_attribute!.IgnoreExtraArgs && args.Length > _parameters.Length)
        {
            if (_logger.IsEnabled(LogLevel.Warning))
            {
                _logger.LogWarning("Invalid number of parameters on command {Name:c}. Expected {ParametersLength}, got {ArgsLength}", Name, _parameters.Length, args.Length);
            }

            return;
        }

        object?[] parameters = new object[_parameters.Length];

        for (var i = 0; i < _parameters.Length; i++)
        {
            ParameterInfo paramInfo = _parameters[i];

            if (i < args.Length)
            {
                parameters[i] = GetParameterValue(_parameters[i], args[i]);

                if (parameters[i] == null && !paramInfo.ParameterType.IsClass) return;
            }
            else if (paramInfo.HasDefaultValue)
            {
                parameters[i] = paramInfo.DefaultValue;
            }
        }

        object? result = _handlerMethod!.Invoke(this, parameters);

        if (result is Task task)
        {
            await task;
        }
    }

    /// <summary>
    /// Converts a string argument into the type required by a command handler parameter.
    /// </summary>
    ///
    /// <param name="parameterInfo">Metadata for the target command handler parameter.</param>
    /// <param name="argument">The string argument to convert to the target parameter type.</param>
    ///
    /// <returns>The converted argument value, or <c>null</c> when conversion fails and a warning has been logged.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private object? GetParameterValue(ParameterInfo parameterInfo, string argument)
    {
        Type parameterType = parameterInfo.ParameterType;

        if (parameterType.IsEnum)
        {
            try
            {
                return Enum.Parse(parameterType, argument, true);
            }
            catch (Exception ex) when (ex is ArgumentNullException or ArgumentException or OverflowException or InvalidCastException)
            {
                if (_logger.IsEnabled(LogLevel.Warning))
                {
                    _logger.LogWarning("Invalid enum value {Value:b} for parameter {ParamName:b}. Valid values are: {ValidValues:c}",
                        argument, parameterInfo.Name, string.Join(", ", Enum.GetNames(parameterType)));
                }

                return null;
            }
        }

        try
        {
            return Convert.ChangeType(argument, Nullable.GetUnderlyingType(parameterType) ?? parameterType);
        }
        catch (Exception ex) when (ex is InvalidCastException or FormatException or OverflowException or ArgumentNullException)
        {
            if (_logger.IsEnabled(LogLevel.Warning))
            {
                _logger.LogWarning("Cannot convert value {Value:b} to type {Type:c} for parameter {ParamName:b}",
                    argument, parameterType.Name, parameterInfo.Name);
            }

            return null;
        }
    }
}
