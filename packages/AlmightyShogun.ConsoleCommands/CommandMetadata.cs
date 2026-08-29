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

        if (!IsInvocableName(declaredAttribute.Name))
        {
            error = $"{commandType.Name} declares the command name '{declaredAttribute.Name}', which cannot be typed at the "
                    + "prompt. A name must not be blank and must contain no whitespace, because input is split on spaces.";

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

        if (!IsAwaitableReturn(handlerMethods[0].ReturnType))
        {
            error = $"{commandType.Name}.ExecuteAsync must return {nameof(Task)} or {nameof(ValueTask)}. A command is only "
                    + "ever invoked by someone typing it at the prompt, so there is nowhere for a return value to go.";

            return false;
        }

        attribute = declaredAttribute;
        handlerMethod = handlerMethods[0];

        return true;
    }

    /// <summary>
    /// Checks whether a declared name is one a user could actually type and the dispatcher could actually match.
    /// </summary>
    ///
    /// <param name="name">The name from the class attribute, or from an alias.</param>
    ///
    /// <returns><c>true</c> when the name is non-blank and free of whitespace; otherwise <c>false</c>.</returns>
    ///
    /// <remarks>
    /// Input is split on spaces before the first token is looked up, so a name containing one can never be matched however
    /// it is typed. Rejecting it here is what stops such a command registering and then never responding.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal static bool IsInvocableName(string name) => !string.IsNullOrWhiteSpace(name) && !name.Any(char.IsWhiteSpace);

    /// <summary>
    /// Checks whether a handler's return type is one the dispatcher can await.
    /// </summary>
    ///
    /// <param name="returnType">The declared return type of <c>ExecuteAsync</c>.</param>
    ///
    /// <returns><c>true</c> for <see cref="Task"/> and <see cref="ValueTask"/>; otherwise <c>false</c>.</returns>
    ///
    /// <remarks>
    /// The generic forms are rejected along with everything else. A command's result has nowhere to go, so returning one
    /// is a sign the method was meant to be called by something other than the prompt.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal static bool IsAwaitableReturn(Type returnType) => returnType == typeof(Task) || returnType == typeof(ValueTask);
}
