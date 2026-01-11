using System.Reflection;
using Microsoft.Extensions.Logging;

namespace AlmightyShogun.ConsoleCommands;

public abstract class ConsoleCommandBase : IConsoleCommand
{
    public required string Name { get; init; }
    public string? Usage { get; }
    public required string Description { get; init; }
    public IReadOnlyList<string> Aliases => _aliases;

    private readonly List<string> _aliases = [];
    private readonly MethodInfo? _handlerMethod;
    private readonly ParameterInfo[] _parameters = [];
    private readonly ILogger<ConsoleCommandBase> _logger;
    private readonly ConsoleCommandAttribute? _attribute;

    protected ConsoleCommandBase(ILogger<ConsoleCommandBase> logger)
    {
        _logger = logger;

        _handlerMethod = GetType()
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .First(m => m.GetCustomAttribute<ConsoleCommandAttribute>() is not null);

        if (_handlerMethod is not null)
        {
            _attribute = _handlerMethod.GetCustomAttribute<ConsoleCommandAttribute>();

            if (_attribute is not null)
            {
                Name = _attribute.Name;
                Description = _attribute.Description;
            }
            
            var aliasAttribute = _handlerMethod.GetCustomAttribute<AliasAttribute>();
        
            _aliases.AddRange(aliasAttribute?.Aliases ?? []);

            _parameters = _handlerMethod.GetParameters();
            
            return;
        }

        if (_logger.IsEnabled(LogLevel.Warning))
        {
            _logger.LogWarning("No method with ConsoleCommandAttribute found in {Name:y}", GetType().Name);
        }
    }

    /// <summary>
    /// Handles the execution of a console command with the provided arguments.
    /// </summary>
    /// 
    /// <param name="args">An array of strings representing the arguments passed to the command. Arguments must match the command's parameter configuration.</param>
    ///
    /// <returns>A task that represents the asynchronous operation of handling the command.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public async Task InternallyExecuteCommandAsync(string[] args)
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
    /// Converts a string argument into the appropriate type for a command parameter.
    /// This includes handling standard types, enum conversion, and logging warnings for invalid conversions.
    /// </summary>
    /// 
    /// <param name="parameterInfo">Metadata about the parameter, including its type and attributes.</param>
    /// <param name="argument">The string argument to convert to the target parameter type.</param>
    /// 
    /// <returns>The converted value as an object, or null if the conversion fails.</returns>
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
