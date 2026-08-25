using System.Reflection;

namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Validates that a type is a usable console command, so the base constructor and the assembly scanner agree on what
/// valid means while differing on what to do about invalid: the constructor throws, the scanner skips.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class CommandMetadata
{
    /// <summary>
    /// Checks a candidate command type against all three rules the dispatcher depends on, and reports the first one it
    /// breaks in a sentence fit to put straight into an exception message.
    /// </summary>
    ///
    /// <param name="commandType">The candidate type, already known to be concrete.</param>
    /// <param name="attribute">
    /// The class attribute on success. Left <c>null</c> on failure despite the non-nullable type, so a caller that ignores
    /// the result gets a null reference rather than a usable value.
    /// </param>
    /// <param name="handlerMethod">The sole public <c>ExecuteAsync</c> on success, left <c>null</c> on failure as above.</param>
    /// <param name="error">The reason it was rejected, naming the type; <c>null</c> on success.</param>
    ///
    /// <returns><c>true</c> when the type can be dispatched to; otherwise <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
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

        if (handlerMethods[0].ReturnType != typeof(Task))
        {
            error = $"{commandType.Name}.ExecuteAsync must return {nameof(Task)}. A command is only ever invoked by someone "
                    + "typing it at the prompt, so there is nowhere for a return value to go.";

            return false;
        }

        attribute = declaredAttribute;
        handlerMethod = handlerMethods[0];

        return true;
    }
}
