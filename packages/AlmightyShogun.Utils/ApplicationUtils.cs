using System.Reflection;

namespace AlmightyShogun.Utils;

public static class ApplicationUtils
{
    /// <summary>
    /// Sets the title of the console window.
    /// </summary>
    /// 
    /// <param name="title">The title to set for the console window.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public static void Title(string title) => Console.Title = title;

    /// <summary>
    /// Retrieves a collection of types that inherit from the specified base type or implement the specified interface within the provided assemblies.
    /// </summary>
    /// 
    /// <typeparam name="T">The base type or interface to check for inheritance or implementation.</typeparam>
    /// <param name="assemblies">An array of assemblies to search for matching types.</param>
    /// 
    /// <returns>A collection of types that inherit from the specified base type or implement the specified interface.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public static IEnumerable<Type> GetOnInherit<T>(params Assembly[] assemblies)
    {
        if (assemblies.Length == 0)
        {
            assemblies = [Assembly.GetCallingAssembly()];
        }
        
        return assemblies.SelectMany(a => a.GetTypes()).Where(t => typeof(T).IsAssignableFrom(t));
    }

    /// <summary>
    /// Prevents the application from shutting down when Ctrl+C is pressed.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.1.0</since>
    public static void PreventCancellation()
    {
        Console.CancelKeyPress += (s, e) =>
        {
            e.Cancel = true;
        };
    }
}
