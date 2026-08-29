using System.Reflection;
using System.Globalization;
using System.ComponentModel;
using Microsoft.Extensions.Logging;

namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Turns the strings typed at the prompt into the values a handler's parameters are declared as. Everything arrives as
/// text, so this is the only place a command's typed signature is reconciled with what the user actually wrote.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class CommandArgumentBinder
{
    /// <summary>
    /// Checks the argument count against the parameters before any conversion is attempted, so a plainly wrong line is
    /// rejected without the cost of parsing it.
    /// </summary>
    ///
    /// <param name="parameters">The handler parameters, with any trailing cancellation token already removed.</param>
    /// <param name="argumentCount">The number of tokens typed after the command name.</param>
    /// <param name="ignoreExtraArguments">
    /// Whether surplus tokens are tolerated. It relaxes only the upper bound; a line short of the required parameters is
    /// rejected either way.
    /// </param>
    ///
    /// <returns><c>true</c> when the count could fill the parameters; otherwise <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal static bool IsArgumentCountValid(ParameterInfo[] parameters, int argumentCount, bool ignoreExtraArguments)
    {
        int required = parameters.Count(parameter => !parameter.HasDefaultValue);

        return argumentCount >= required && (ignoreExtraArguments || argumentCount <= parameters.Length);
    }

    /// <summary>
    /// Converts each argument to its parameter's type, matching them positionally and filling any parameter the user
    /// stopped short of with its declared default.
    /// </summary>
    ///
    /// <param name="parameters">The handler parameters, with any trailing cancellation token already removed.</param>
    /// <param name="arguments">The tokens typed after the command name, which may be fewer than the parameters.</param>
    /// <param name="logger">
    /// The logger a rejected argument is reported through, naming the parameter and, for an enum, the values that would
    /// have worked.
    /// </param>
    /// <param name="values">
    /// The values to invoke with, positionally aligned to the parameters. Empty when the bind failed, so it is only
    /// meaningful on <c>true</c>.
    /// </param>
    ///
    /// <returns><c>true</c> when every supplied argument converted; otherwise <c>false</c>.</returns>
    ///
    /// <remarks>
    /// A failed conversion aborts the whole bind. Running a command with a defaulted value in place of an argument the
    /// user actually typed is worse than not running it.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal static bool TryBind(ParameterInfo[] parameters, string[] arguments, ILogger logger, out object?[] values)
    {
        values = new object?[parameters.Length];

        for (var index = 0; index < parameters.Length; index++)
        {
            ParameterInfo parameter = parameters[index];

            if (index >= arguments.Length)
            {
                values[index] = parameter.HasDefaultValue ? parameter.DefaultValue : null;

                continue;
            }

            if (!TryConvert(parameter, arguments[index], logger, out object? value))
            {
                values = [];

                return false;
            }

            values[index] = value;
        }

        return true;
    }

    /// <summary>
    /// Converts one token to one parameter's type, unwrapping a nullable to its underlying type first so <c>int?</c> is
    /// parsed exactly as <c>int</c> would be.
    /// </summary>
    ///
    /// <param name="parameter">The parameter being filled, used for its type and for naming it in a complaint.</param>
    /// <param name="argument">The token as typed.</param>
    /// <param name="logger">The logger a rejected argument is reported through.</param>
    /// <param name="value">The converted value, or <c>null</c> when the conversion failed.</param>
    ///
    /// <returns><c>true</c> when the token converted; otherwise <c>false</c>.</returns>
    ///
    /// <remarks>
    /// An enum is matched case-insensitively by name and then checked with <see cref="Enum.IsDefined(Type, object)"/>,
    /// because <see cref="Enum.TryParse(Type, string, bool, out object)"/> also accepts any bare number and would
    /// otherwise let an undefined value through.
    ///
    /// Everything else is tried against <see cref="Convert.ChangeType(object, Type, IFormatProvider)"/> first, which covers
    /// the primitives, and then against the type's <see cref="TypeConverter"/>, which is what makes
    /// <see cref="Guid"/>, <see cref="TimeSpan"/>, <see cref="DateOnly"/>, <see cref="Uri"/> and any type carrying a
    /// <see cref="TypeConverterAttribute"/> bindable. Both run under the invariant culture, so a decimal or a date argument
    /// means the same thing whatever machine the application runs on.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool TryConvert(ParameterInfo parameter, string argument, ILogger logger, out object? value)
    {
        value = null;

        Type parameterType = Nullable.GetUnderlyingType(parameter.ParameterType) ?? parameter.ParameterType;

        if (parameterType.IsEnum)
        {
            if (Enum.TryParse(parameterType, argument, true, out object? parsed) && Enum.IsDefined(parameterType, parsed!))
            {
                value = parsed;

                return true;
            }

            if (logger.IsEnabled(LogLevel.Warning))
                logger.LogWarning(
                    "Invalid enum value {Value:b} for parameter {ParamName:b}. Valid values are: {ValidValues:c}",
                    argument,
                    parameter.Name,
                    string.Join(", ", Enum.GetNames(parameterType))
                );

            return false;
        }

        if (TryChangeType(argument, parameterType, out value) || TryConvertFromString(argument, parameterType, out value))
            return true;

        if (logger.IsEnabled(LogLevel.Warning))
            logger.LogWarning(
                "Cannot convert value {Value:b} to type {Type:c} for parameter {ParamName:b}",
                argument,
                parameterType.Name,
                parameter.Name
            );

        return false;
    }

    /// <summary>
    /// Converts through <see cref="Convert.ChangeType(object, Type, IFormatProvider)"/>, which handles the primitives and
    /// anything else implementing <see cref="IConvertible"/>.
    /// </summary>
    ///
    /// <param name="argument">The token as typed.</param>
    /// <param name="parameterType">The target type, already unwrapped from <see cref="Nullable{T}"/>.</param>
    /// <param name="value">The converted value on success; <c>null</c> otherwise.</param>
    ///
    /// <returns><c>true</c> when the token converted; otherwise <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
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
    /// <param name="argument">The token as typed.</param>
    /// <param name="parameterType">The target type, already unwrapped from <see cref="Nullable{T}"/>.</param>
    /// <param name="value">The converted value on success; <c>null</c> otherwise.</param>
    ///
    /// <returns><c>true</c> when the token converted; otherwise <c>false</c>.</returns>
    ///
    /// <remarks>
    /// Every exception is swallowed rather than the parse-shaped ones alone, because a converter is third-party code and
    /// may throw anything at all to mean "not my format". The caller reports the failure either way.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool TryConvertFromString(string argument, Type parameterType, out object? value)
    {
        value = null;

        TypeConverter converter = TypeDescriptor.GetConverter(parameterType);

        if (!converter.CanConvertFrom(typeof(string)))
            return false;

        try
        {
            value = converter.ConvertFromInvariantString(argument);

            return value is not null;
        }
        catch
        {
            return false;
        }
    }
}
