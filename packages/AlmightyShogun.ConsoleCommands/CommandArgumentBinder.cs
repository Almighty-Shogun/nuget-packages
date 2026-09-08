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
/// <since>4.0.0</since>
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
    /// rejected either way. A handler ending in an array parameter has no upper bound for it to relax.
    /// </param>
    ///
    /// <returns><c>true</c> when the count could fill the parameters; otherwise <c>false</c>.</returns>
    ///
    /// <remarks>
    /// A trailing array parameter takes every token the parameters before it did not, so it lifts the upper bound
    /// altogether and requires no argument of its own: a line stopping short of it is still counted as valid, and
    /// <see cref="TryBind"/> decides what it is filled with.
    /// </remarks>
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
    /// Counts the arguments a line must carry before <see cref="IsArgumentCountValid"/> will accept it, which is every
    /// parameter without a default, less the trailing array parameter when that is the one missing a default.
    /// </summary>
    ///
    /// <param name="parameters">The handler parameters, with any trailing cancellation token already removed.</param>
    ///
    /// <returns>The smallest argument count that could fill the parameters.</returns>
    ///
    /// <remarks>
    /// A trailing array parameter is counted as required by the declaration and yet is filled by an empty array when nothing
    /// is left for it, so it is subtracted back out. One carrying a default is not counted in the first place, which is why
    /// the subtraction is conditional rather than applied to every variadic signature.
    ///
    /// This is the lower bound only. The upper bound belongs to <see cref="IsArgumentCountValid"/>, which is where
    /// <c>ignoreExtraArguments</c> and the absent bound of a variadic tail are decided.
    /// </remarks>
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
    /// Checks whether the last parameter is the one every token left over is collected into, which is how an array in that
    /// position is bound.
    /// </summary>
    ///
    /// <param name="parameters">The handler parameters, with any trailing cancellation token already removed.</param>
    ///
    /// <returns><c>true</c> when the last parameter is a single-dimension array; otherwise <c>false</c>.</returns>
    ///
    /// <remarks>
    /// <see cref="ParamArrayAttribute"/> is not consulted, so a plain array in the last position collects the tail exactly
    /// as a <c>params</c> one does. Only the last position is treated this way; an array declared before another parameter
    /// is matched to a single token like any other type.
    ///
    /// Every element still comes from the input line split on spaces, so no element can carry a space, whatever it is
    /// quoted with.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal static bool HasVariadicTail(ParameterInfo[] parameters)
        => parameters.Length > 0 && parameters[^1].ParameterType.IsSZArray;

    /// <summary>
    /// Converts each argument to its parameter's type, matching them positionally and filling any parameter the user
    /// stopped short of with its declared default. A trailing array parameter takes every token the others left instead of
    /// a single one.
    /// </summary>
    ///
    /// <param name="parameters">The handler parameters, with any trailing cancellation token already removed.</param>
    /// <param name="arguments">
    /// The tokens typed after the command name, which may be fewer than the parameters, and more than them only for a
    /// handler ending in an array parameter or one declared with <c>ignoreExtraArgs</c>.
    /// </param>
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
    /// <exception cref="Exception">
    /// Whatever <see cref="TryConvert"/> let escape, which is a <see cref="TypeConverter"/> failing outside the call
    /// <see cref="TryConvertFromString"/> guards. It is not caught here either, so such an argument leaves the bind by
    /// exception rather than through <c>false</c>, and the dispatcher reports it as a command failure.
    /// </exception>
    ///
    /// <remarks>
    /// A failed conversion aborts the whole bind, so no parameter is filled with its default in place of an argument the
    /// user typed. A surplus token beyond the declared parameters is dropped unless an array tail collects it.
    /// </remarks>
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
    /// Converts the tokens left over into an array of the parameter's element type, which is the value a trailing array
    /// parameter is invoked with.
    /// </summary>
    ///
    /// <param name="parameter">The trailing array parameter, used for its element type and for naming it in a complaint.</param>
    /// <param name="arguments">The tokens from this parameter's position onwards, empty when the line stopped short of it.</param>
    /// <param name="logger">The logger a rejected element is reported through, naming the parameter rather than the position.</param>
    /// <param name="value">
    /// The array to invoke with, or the parameter's declared default when the line supplied nothing and it has one;
    /// <c>null</c> when an element failed to convert.
    /// </param>
    ///
    /// <returns><c>true</c> when every token converted to the element type; otherwise <c>false</c>.</returns>
    ///
    /// <exception cref="Exception">
    /// Whatever <see cref="TryConvert"/> let escape for one of the elements, which passes through here unhandled just as it
    /// does through <see cref="TryBind"/>.
    /// </exception>
    ///
    /// <remarks>
    /// One rejected element fails the whole bind rather than being skipped, so the handler never runs against a tail
    /// shorter than what was typed.
    /// </remarks>
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
        Array tail = Array.CreateInstance(elementType, arguments.Length);

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
    /// Converts one token to one target type, unwrapping a nullable to its underlying type first so <c>int?</c> is parsed
    /// exactly as <c>int</c> would be.
    /// </summary>
    ///
    /// <param name="parameterType">
    /// The type to convert to, which is the parameter's own type, or the element type when the token is one of the values
    /// going into an array tail.
    /// </param>
    /// <param name="parameterName">The name to blame in a complaint, so a rejected token names the parameter it was meant for.</param>
    /// <param name="argument">The token as typed.</param>
    /// <param name="logger">The logger a rejected argument is reported through.</param>
    /// <param name="value">The converted value, or <c>null</c> when the conversion failed.</param>
    ///
    /// <returns><c>true</c> when the token converted; otherwise <c>false</c>.</returns>
    ///
    /// <exception cref="Exception">
    /// Whatever <see cref="TryConvertFromString"/> let escape, which is a <see cref="TypeConverter"/> failing outside the
    /// call that method guards. Nothing here catches it, so the token ends the bind instead of being rejected.
    /// </exception>
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
    /// <param name="argument">The token as typed.</param>
    /// <param name="parameterType">The target type, already unwrapped from <see cref="Nullable{T}"/>.</param>
    /// <param name="value">The converted value on success; <c>null</c> otherwise.</param>
    ///
    /// <returns><c>true</c> when the token converted; otherwise <c>false</c>.</returns>
    ///
    /// <exception cref="Exception">
    /// Whatever <c>TypeDescriptor.GetConverter</c> or <c>CanConvertFrom</c> raised. Both run before the <c>try</c>, so a
    /// converter failing there is not turned into <c>false</c> the way one failing inside the conversion itself is.
    /// </exception>
    ///
    /// <remarks>
    /// Every exception out of <c>ConvertFromInvariantString</c> is swallowed rather than the parse-shaped ones alone,
    /// because a converter is third-party code and may throw anything at all to mean "not my format". The caller reports
    /// the failure either way.
    ///
    /// Only that call sits inside the catch. <c>TypeDescriptor.GetConverter</c> and <c>CanConvertFrom</c> run before it, so
    /// the swallowing covers the conversion alone and not the two calls that select the converter.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
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
