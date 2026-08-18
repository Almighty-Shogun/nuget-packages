using System.Reflection;
using System.Runtime.CompilerServices;

namespace AlmightyShogun.Utils;

/// <summary>
/// Provides the console and reflection primitives the other helpers in this package build on: setting the window title,
/// suppressing interactive cancellation, and discovering implementation types across assemblies. Every member is static and
/// the type is never registered in a container.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
public static class ApplicationUtils
{
    /// <summary>
    /// Tracks whether the cancellation handler has already been attached, so that
    /// <see cref="PreventCancellation"/> stays idempotent instead of stacking one handler per call.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static bool _cancellationPrevented;

    /// <summary>
    /// Sets the console window title. Behavior is platform and terminal dependent: a terminal that does not support titles
    /// discards the value rather than reporting an error, so this is presentation only and never worth branching on.
    /// </summary>
    ///
    /// <param name="title">
    /// The text to show as the window title. Passed through unchanged, so any truncation or escaping is the terminal's.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public static void Title(string title) => Console.Title = title;

    /// <summary>
    /// Retrieves the concrete types in the specified assemblies that inherit from or implement <typeparamref name="T"/>.
    /// Interfaces and abstract classes are excluded, so every returned type can be instantiated.
    /// </summary>
    ///
    /// <typeparam name="T">
    /// The base type or interface to match. Assignability is used, so an indirect subclass or an interface implemented by a
    /// base class matches just as well as a direct one.
    /// </typeparam>
    /// <param name="assemblies">
    /// The assemblies to scan. If none are provided the calling assembly is used, which is why this method must not be
    /// inlined into its caller.
    /// </param>
    ///
    /// <returns>
    /// The matching concrete types, in assembly then declaration order. The sequence is lazy, so the reflection work happens
    /// as it is enumerated rather than when this method returns.
    /// </returns>
    ///
    /// <remarks>
    /// This is a raw reflection primitive with no dependency-injection semantics. It does not honor
    /// <see cref="SkipAutoRegistrationAttribute"/>; only the registration helpers do. An assembly whose types cannot all be
    /// loaded contributes the types that did load, so one unresolvable dependency does not end the scan.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public static IEnumerable<Type> GetOnInherit<T>(params Assembly[] assemblies)
    {
        if (assemblies.Length == 0)
            assemblies = [Assembly.GetCallingAssembly()];

        return assemblies
            .SelectMany(SafeGetTypes)
            .Where(t => typeof(T).IsAssignableFrom(t) && t is { IsInterface: false, IsAbstract: false });
    }

    /// <summary>
    /// Reads the types defined in an assembly, keeping the ones that loaded when others could not.
    /// </summary>
    ///
    /// <param name="assembly">The assembly to read the defined types from.</param>
    ///
    /// <returns>
    /// Every type the assembly defines, or the subset that loaded when a dependency could not be resolved.
    /// </returns>
    ///
    /// <remarks>
    /// A partially loadable assembly throws <see cref="ReflectionTypeLoadException"/> from
    /// <see cref="Assembly.GetTypes"/> while still exposing the types that succeeded on the exception itself. Discovery is a
    /// best-effort scan, so the usable subset is preferable to failing the whole call over one missing dependency.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static IEnumerable<Type> SafeGetTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException exception)
        {
            return exception.Types.Where(type => type is not null)!;
        }
    }

    /// <summary>
    /// Stops <c>Ctrl+C</c> from terminating the process, so a long-running console application can decide for itself when to
    /// shut down. Calling it more than once has no additional effect.
    /// </summary>
    ///
    /// <remarks>
    /// There is no counterpart that restores the default. Once suppressed, cancellation stays suppressed for the life of the
    /// process, and the application must expose some other way to stop. A hosted application should prefer
    /// <c>UseCustomConsoleLifetime</c> from <c>AlmightyShogun.Hosting.Utils</c>, which suppresses the same key press but still
    /// shuts down cleanly on <c>SIGTERM</c>.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.1.0</since>
    public static void PreventCancellation()
    {
        if (_cancellationPrevented) return;

        _cancellationPrevented = true;

        Console.CancelKeyPress += (s, e) =>
        {
            e.Cancel = true;
        };
    }
}
