using System.Reflection;

namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Validates and describes console command types.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal static class CommandMetadata
{
    /// <summary>
    /// Validates a command type and returns its command attribute and handler method.
    /// </summary>
    ///
    /// <param name="commandType">The command type to validate.</param>
    /// <param name="attribute">
    /// The command attribute when validation succeeds.
    /// </param>
    /// <param name="handlerMethod">The validated <c>ExecuteAsync</c> method when validation succeeds.</param>
    /// <param name="error">The validation error when validation fails.</param>
    ///
    /// <returns><c>true</c> when the command type is valid; otherwise <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    internal static bool TryDescribe(
        Type commandType,
        out ConsoleCommandAttribute attribute,
        out MethodInfo handlerMethod,
        out string? error
    )
    {
        error = null;
        attribute = null!;
        handlerMethod = null!;

        var declaredAttribute = commandType.GetCustomAttribute<ConsoleCommandAttribute>();

        if (declaredAttribute is null)
        {
            error = $"{commandType.Name} must define {nameof(ConsoleCommandAttribute)} on the class.";

            return false;
        }

        if (!IsInvocableName(declaredAttribute.Name))
        {
            error = $"{commandType.Name} declares the command name '{declaredAttribute.Name}', which cannot be typed at the "
                    + "prompt. A name must not be blank and must contain no whitespace, because input is split on spaces.";

            return false;
        }

        var aliasAttribute = commandType.GetCustomAttribute<AliasAttribute>();

        if (aliasAttribute is not null)
        {
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                declaredAttribute.Name
            };

            foreach (string alias in aliasAttribute.Aliases)
            {
                if (!IsInvocableName(alias))
                {
                    error = $"{commandType.Name} declares the alias '{alias}', which cannot be typed at the prompt. "
                            + "An alias must not be blank and must contain no whitespace.";

                    return false;
                }

                if (names.Add(alias)) continue;
                error = $"{commandType.Name} declares the command name or alias '{alias}' more than once.";

                return false;
            }
        }

        MethodInfo[] handlerMethods =
        [
            .. commandType
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(method => string.Equals(method.Name, "ExecuteAsync", StringComparison.Ordinal))
        ];

        if (handlerMethods.Length != 1)
        {
            error = $"{commandType.Name} must define exactly one public instance method named ExecuteAsync.";

            return false;
        }

        MethodInfo declaredHandlerMethod = handlerMethods[0];

        if (!IsAwaitableReturn(declaredHandlerMethod.ReturnType))
        {
            error = $"{commandType.Name}.ExecuteAsync must return {nameof(Task)} or {nameof(ValueTask)}. A command is only "
                    + "ever invoked by someone typing it at the prompt, so there is nowhere for a return value to go.";

            return false;
        }

        if (declaredHandlerMethod.IsGenericMethodDefinition)
        {
            error = $"{commandType.Name}.ExecuteAsync must not be generic.";

            return false;
        }

        ParameterInfo[] parameters = declaredHandlerMethod.GetParameters();

        if (parameters.Any(parameter => parameter.ParameterType.IsByRef))
        {
            error = $"{commandType.Name}.ExecuteAsync cannot declare ref, out, or in parameters.";
            return false;
        }

        for (var index = 0; index < parameters.Length; index++)
        {
            ParameterInfo parameter = parameters[index];
            if (parameter.ParameterType == typeof(CancellationToken) && index != parameters.Length - 1)
            {
                error = $"{commandType.Name}.ExecuteAsync may only declare CancellationToken as its final parameter.";
                return false;
            }

            if (parameter.ParameterType.IsArray
                && !parameter.IsDefined(typeof(ParamArrayAttribute), false))
            {
                error = $"{commandType.Name}.ExecuteAsync cannot declare array parameters unless they use params.";
                return false;
            }
        }

        attribute = declaredAttribute;
        handlerMethod = declaredHandlerMethod;

        return true;
    }

    /// <summary>
    /// Validates a command type and returns its metadata.
    /// </summary>
    ///
    /// <param name="commandType">The command type to validate.</param>
    ///
    /// <returns>The command attribute and handler method.</returns>
    ///
    /// <exception cref="InvalidOperationException">
    /// Thrown when the command type is invalid.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    internal static (ConsoleCommandAttribute Attribute, MethodInfo HandlerMethod) Describe(Type commandType)
        => !TryDescribe(commandType, out ConsoleCommandAttribute attribute, out MethodInfo handlerMethod, out string? error)
            ? throw new InvalidOperationException(error)
            : (attribute, handlerMethod);

    /// <summary>
    /// Determines whether a command name can be matched from console input.
    /// </summary>
    ///
    /// <param name="name">The command name or alias to validate</param>
    ///
    /// <returns>><c>true</c> when the name is non-blank and contains no whitespace; otherwise <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    internal static bool IsInvocableName(string name) => !string.IsNullOrWhiteSpace(name) && !name.Any(char.IsWhiteSpace);

    /// <summary>
    /// Determines whether a command handler return type is supported.
    /// </summary>
    ///
    /// <param name="returnType">The handler return type.</param>
    ///
    /// <returns><c>true</c> for <see cref="Task"/> and <see cref="ValueTask"/>; otherwise <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    internal static bool IsAwaitableReturn(Type returnType) => returnType == typeof(Task) || returnType == typeof(ValueTask);
}
