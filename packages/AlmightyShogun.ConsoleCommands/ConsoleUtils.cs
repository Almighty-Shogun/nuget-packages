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
    /// Retrieves all commands marked with the <see cref="ConsoleCommandAttribute"/> defined within the application.
    /// </summary>
    /// 
    /// <returns>A list of <see cref="ConsoleCommand"/> instances representing the registered commands.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public static List<ConsoleCommand> GetAllCommands() => AppDomain.CurrentDomain.GetAssemblies()
        .SelectMany(a => a.GetTypes())
        .Where(t => typeof(IConsoleCommand).IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false })
        .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance))
        .Where(m => m.GetCustomAttribute<ConsoleCommandAttribute>() is not null)
        .Select(m => new ConsoleCommand(
            m.GetCustomAttribute<ConsoleCommandAttribute>()!.Name,
            m.GetCustomAttribute<ConsoleCommandAttribute>()!.Description,
            m.GetCustomAttribute<AliasAttribute>()?.Aliases ?? [],
            string.Join(" ", m.GetParameters().Select(p => $"<{p.Name}:{p.ParameterType.Name}>")),
            m.GetCustomAttribute<ExampleAttribute>()?.Example ?? null
        )).ToList();
}
