using System.Reflection;
using System.Globalization;
using System.ComponentModel;
using Microsoft.Extensions.Logging;

namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Binds parsed command arguments to handler parameters.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal static class CommandArgumentBinder
{
    /// <summary>
    /// Determines whether an argument count can satisfy the supplied parameters.
    /// </summary>
    ///
    /// <param name="parameters">The command parameters, excluding a trailing cancellation token.</param>
    /// <param name="argumentCount">The number of parsed arguments.</param>
    /// <param name="ignoreExtraArguments">Whether surplus arguments are allowed.</param>
    ///
    /// <returns><c>true</c> when the argument count is valid; otherwise <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    internal static bool IsArgumentCountValid(ParameterInfo[] parameters, int argumentCount, bool ignoreExtraArguments)
    {
        int required = GetMinimumArgumentCount(parameters);

        if (!HasVariadicTail(parameters))
            return argumentCount >= required && (ignoreExtraArguments || argumentCount <= parameters.Length);

        return argumentCount >= required;
    }

    /// <summary>
    /// Gets the minimum number of arguments required by the supplied parameters.
    /// </summary>
    ///
    /// <param name="parameters">The command parameters, excluding a trailing cancellation token.</param>
    ///
    /// <returns>The minimum required argument count.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal static int GetMinimumArgumentCount(ParameterInfo[] parameters)
    {
        int required = parameters.Count(parameter => !parameter.HasDefaultValue);

        if (!HasVariadicTail(parameters))
            return required;

        return parameters[^1].HasDefaultValue ? required : required - 1;
    }

    /// <summary>
    /// Determines whether the final parameter is a <c>params</c> array.
    /// </summary>
    ///
    /// <param name="parameters">The command parameters.</param>
    ///
    /// <returns><c>true</c> when the final parameter is a <c>params</c> array; otherwise <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal static bool HasVariadicTail(ParameterInfo[] parameters)
        => parameters.Length > 0 && parameters[^1].IsDefined(typeof(ParamArrayAttribute) , false);

    /// <summary>
    /// Binds parsed arguments to command parameters.
    /// </summary>
    ///
    /// <param name="parameters">The command parameters, excluding a trailing cancellation token.</param>
    /// <param name="arguments">
    /// The parsed command arguments.
    /// </param>
    /// <param name="logger">
    /// The logger used to report conversion failures.
    /// </param>
    /// <param name="values">
    /// The bound parameter values when binding succeeds.
    /// </param>
    ///
    /// <returns><c>true</c> when all arguments were bound successfully; otherwise <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    internal static bool TryBind(ParameterInfo[] parameters, string[] arguments, ILogger logger, out object?[] values)
    {
        values = new object?[parameters.Length];

        bool hasVariadicTail = HasVariadicTail(parameters);

        for (var index = 0; index < parameters.Length; index++)
        {
            ParameterInfo parameter = parameters[index];

            if (hasVariadicTail && index == parameters.Length - 1)
            {
                string[] tailArguments = index < arguments.Length ? arguments[index..] : [];

                if (!TryBindTail(parameter, tailArguments, logger, out object? tail))
                {
                    values = [];

                    return false;
                }

                values[index] = tail;

                continue;
            }

            if (index >= arguments.Length)
            {
                values[index] = parameter.HasDefaultValue ? parameter.DefaultValue : null;

                continue;
            }

            if (!TryConvert(parameter.ParameterType, parameter.Name, arguments[index], logger, out object? value))
            {
                values = [];

                return false;
            }

            values[index] = value;
        }

        return true;
    }

    /// <summary>
    /// Binds remaining arguments to a <c>params</c> array parameter.
    /// </summary>
    ///
    /// <param name="parameter">The variadic parameter.</param>
    /// <param name="arguments">The remaining arguments.</param>
    /// <param name="logger">The logger used to report conversion failures.</param>
    /// <param name="value">The bound array value when binding succeeds.</param>
    ///
    /// <returns><c>true</c> when every element was bound successfully; otherwise <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool TryBindTail(ParameterInfo parameter, string[] arguments, ILogger logger, out object? value)
    {
        if (arguments.Length == 0 && parameter.HasDefaultValue)
        {
            value = parameter.DefaultValue;

            return true;
        }

        Type elementType = parameter.ParameterType.GetElementType()!;
        var tail = Array.CreateInstance(elementType, arguments.Length);

        for (var index = 0; index < arguments.Length; index++)
        {
            if (!TryConvert(elementType, parameter.Name, arguments[index], logger, out object? element))
            {
                value = null;

                return false;
            }

            tail.SetValue(element, index);
        }

        value = tail;

        return true;
    }

    /// <summary>
    /// Converts one argument to the requested parameter type using invariant culture.
    /// </summary>
    ///
    /// <param name="parameterType">The target parameter type.</param>
    /// <param name="parameterName">The parameter name used when reporting failures.</param>
    /// <param name="argument">The argument to convert.</param>
    /// <param name="logger">The logger used to report conversion failures.</param>
    /// <param name="value">The converted value when conversion succeeds.</param>
    ///
    /// <returns><c>true</c> when conversion succeeds; otherwise <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static bool TryConvert(Type parameterType, string? parameterName, string argument, ILogger logger, out object? value)
    {
        value = null;

        Type targetType = Nullable.GetUnderlyingType(parameterType) ?? parameterType;

        if (targetType.IsEnum)
        {
            if (Enum.TryParse(targetType, argument, true, out object? parsed) && Enum.IsDefined(targetType, parsed!))
            {
                value = parsed;

                return true;
            }

            if (logger.IsEnabled(LogLevel.Warning))
                logger.LogWarning(
                    "Invalid enum value {Value:b} for parameter {ParamName:b}. Valid values are: {ValidValues:c}",
                    argument,
                    parameterName,
                    string.Join(", ", Enum.GetNames(targetType))
                );

            return false;
        }

        if (TryChangeType(argument, targetType, out value) || TryConvertFromString(argument, targetType, out value))
            return true;

        if (logger.IsEnabled(LogLevel.Warning))
            logger.LogWarning(
                "Cannot convert value {Value:b} to type {Type:c} for parameter {ParamName:b}",
                argument,
                targetType.Name,
                parameterName
            );

        return false;
    }

    /// <summary>
    /// Attempts to convert an argument using
    /// <see cref="Convert.ChangeType(object, Type, IFormatProvider)"/> with invariant culture.
    /// </summary>
    ///
    /// <param name="argument">The argument to convert.</param>
    /// <param name="parameterType">The target type.</param>
    /// <param name="value">>The converted value when conversion succeeds; otherwise <c>null</c>.</param>
    ///
    /// <returns><c>true</c> when conversion succeeds; otherwise <c>false</c>.</returns>
    ///
    /// <remarks>
    /// Conversion failures caused by <see cref="InvalidCastException"/>, <see cref="FormatException"/>,
    /// <see cref="OverflowException"/>, or <see cref="ArgumentNullException"/> are treated as unsuccessful conversions.
    /// Other exceptions are allowed to propagate.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static bool TryChangeType(string argument, Type parameterType, out object? value)
    {
        try
        {
            value = Convert.ChangeType(argument, parameterType, CultureInfo.InvariantCulture);

            return true;
        }
        catch (Exception exception)
            when (exception is InvalidCastException or FormatException or OverflowException or ArgumentNullException)
        {
            value = null;

            return false;
        }
    }

    /// <summary>
    /// Converts through the type's <see cref="TypeConverter"/>, which is what a type outside <see cref="IConvertible"/>
    /// declares its own string parsing with.
    /// </summary>
    ///
    /// <param name="argument">The argument to convert</param>
    /// <param name="parameterType">The target type.</param>
    /// <param name="value">The converted value when conversion succeeds.</param>
    ///
    /// <returns><c>true</c> when conversion succeeds; otherwise <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static bool TryConvertFromString(string argument, Type parameterType, out object? value)
    {
        value = null;
        
        try
        {
            TypeConverter converter = TypeDescriptor.GetConverter(parameterType);

            if (!converter.CanConvertFrom(typeof(string)))
                return false;
            
            value = converter.ConvertFromInvariantString(argument);

            return value is not null;
        }
        catch
        {
            return false;
        }
    }
}
