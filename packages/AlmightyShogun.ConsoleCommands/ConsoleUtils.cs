using System.Reflection;

namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Provides console input helpers and command metadata discovery utilities.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
public static class ConsoleUtils
{
    /// <summary>
    /// Removes the previous console line when the current console host supports cursor movement.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static void RemoveLastLine()
    {
        if (Console.IsOutputRedirected || Console.CursorTop <= 0)
            return;

        try
        {
            int line = Console.CursorTop - 1;

            Console.SetCursorPosition(0, line);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, line);
        }
        catch (Exception exception) when (exception is IOException or ArgumentOutOfRangeException or InvalidOperationException)
        {
            // Some hosts do not allow cursor movement even when output is not reported as redirected.
        }
    }

    /// <summary>
    /// Prompts the user with a question and waits until a value or default answer is available.
    /// </summary>
    ///
    /// <param name="question">The question to display to the user in the console.</param>
    /// <param name="defaultValue">The value to use when the user submits an empty answer.</param>
    ///
    /// <returns>A task containing the user's answer or the configured default value.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public static async Task<string> AskQuestionAsync(string question, string? defaultValue = null)
    {
        string? answer = null;

        while (answer is null)
        {
            await Console.Out.WriteAsync($"[QUESTION] {question}: ");
            Console.ForegroundColor = ConsoleColor.Blue;

            string input = Console.ReadLine() ?? "";

            answer = input.Length >= 1 ? input : defaultValue;

            Console.ResetColor();
            RemoveLastLine();
        }

        return answer;
    }

    /// <summary>
    /// Retrieves command types marked with <see cref="ConsoleCommandAttribute"/> from the provided assemblies.
    /// </summary>
    ///
    /// <param name="assemblies">The assemblies to scan. If none are provided, all currently loaded application domain assemblies are used.</param>
    ///
    /// <returns>The concrete command types that implement the internal command contract and expose one public <c>ExecuteAsync</c> method returning <see cref="Task"/>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>3.0.0</since>
    internal static IEnumerable<Type> GetConsoleCommandTypes(params Assembly[] assemblies)
    {
        if (assemblies.Length == 0)
        {
            assemblies = AppDomain.CurrentDomain.GetAssemblies();
        }

        return assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => t is { IsInterface: false, IsAbstract: false } && typeof(IConsoleCommand).IsAssignableFrom(t))
            .Where(t => t.GetCustomAttribute<ConsoleCommandAttribute>() is not null)
            .Where(type =>
            {
                MethodInfo[] executeMethods = type
                    .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(method => string.Equals(method.Name, "ExecuteAsync", StringComparison.Ordinal))
                    .ToArray();

                return executeMethods.Length == 1 && executeMethods[0].ReturnType == typeof(Task);
            });
    }

    /// <summary>
    /// Retrieves metadata for all discovered console commands.
    /// </summary>
    ///
    /// <returns>A list of command metadata records built from command attributes, aliases, examples, and handler parameters.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public static List<ConsoleCommand> GetAllCommands() => GetConsoleCommandTypes()
        .Select(type => new
        {
            Type = type,
            Attribute = type.GetCustomAttribute<ConsoleCommandAttribute>()!,
            Alias = type.GetCustomAttribute<AliasAttribute>(),
            Example = type.GetCustomAttribute<ExampleAttribute>(),
            ExecuteMethod = type.GetMethod("ExecuteAsync", BindingFlags.Public | BindingFlags.Instance)!
        })
        .Select(command => new ConsoleCommand(
            command.Attribute.Name,
            command.Attribute.Description,
            command.Alias?.Aliases ?? [],
            string.Join(" ", command.ExecuteMethod.GetParameters().Select(p => $"<{p.Name}:{p.ParameterType.Name}>")),
            command.Example?.Example ?? null
        )).ToList();
}
