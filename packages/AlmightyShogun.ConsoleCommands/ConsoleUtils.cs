using System.Reflection;

namespace AlmightyShogun.ConsoleCommands;

public static class ConsoleUtils
{
    /// <summary>
    /// Removes the last line displayed on the console by clearing its content
    /// and resetting the cursor position to the beginning of the cleared line.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public static void RemoveLastConsoleLine()
    {
        Console.SetCursorPosition(0, Console.CursorTop - 1);
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write(new string(' ', Console.WindowWidth)); 
        Console.SetCursorPosition(0, Console.CursorTop);
    }

    /// <summary>
    /// Asynchronously prompts the user with a question in the console and waits for a valid non-empty input.
    /// </summary>
    /// 
    /// <param name="question">The question to display to the user in the console.</param>
    /// <param name="defaultValue">The default value if the user does not provide input.</param>
    /// 
    /// <returns>A task representing the asynchronous operation, containing the user's input as a string.</returns>
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
            RemoveLastConsoleLine();
        }

        return answer;
    }

    /// <summary>
    /// Retrieves all command types marked with the <see cref="ConsoleCommandAttribute"/> defined within the application.
    /// </summary>
    /// 
    /// <param name="assemblies">The assemblies to scan for console command types. If not specified, all loaded assemblies are used.</param>
    /// 
    /// <returns>A list of <see cref="Type"/> instances representing the registered console command types.</returns>
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
    /// Retrieves all commands marked with the <see cref="ConsoleCommandAttribute"/> defined within the application.
    /// </summary>
    /// 
    /// <returns>A list of <see cref="ConsoleCommand"/> instances representing the registered commands.</returns>
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
