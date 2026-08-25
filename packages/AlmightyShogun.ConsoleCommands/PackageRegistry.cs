using System.Reflection;
using AlmightyShogun.Core;
using Microsoft.Extensions.DependencyInjection;

namespace AlmightyShogun.ConsoleCommands;

/// <summary>
/// Registers console command services and command implementations in dependency injection.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.1.0</since>
public static class PackageRegistry
{
    /// <param name="serviceCollection">
    /// The service collection the console command services and command classes are registered into.
    /// </param>
    extension(IServiceCollection serviceCollection)
    {
        /// <summary>
        /// Registers the console command handler service. It only runs the input loop, so the command classes it dispatches to
        /// still have to be registered with <see cref="RegisterConsoleCommands(IServiceCollection)"/>.
        /// </summary>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with the console command handler registered.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.0.0</since>
        public IServiceCollection AddConsoleCommands() => serviceCollection.AddSingleton<IConsoleCommandHandler, ConsoleCommandHandler>();

        /// <summary>
        /// Registers the command classes declared in the calling assembly, which is the usual case when the commands live in
        /// the startup project. Reach for the overload taking assemblies when they do not.
        /// </summary>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with the console commands registered.</returns>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>Unreleased</since>
        public IServiceCollection RegisterConsoleCommands() => serviceCollection.RegisterConsoleCommands([Assembly.GetCallingAssembly()]);

        /// <summary>
        /// Registers the command classes declared in the given assemblies as transient services, so a fresh instance is built
        /// for each invocation and a command may depend on scoped application services.
        /// </summary>
        ///
        /// <param name="assemblies">
        /// The assemblies to scan, in the order they should be searched. An empty array registers nothing; the overload taking
        /// no assembly at all is the one that falls back to the calling assembly.
        /// </param>
        ///
        /// <returns>The <see cref="IServiceCollection"/> instance with the console commands registered.</returns>
        ///
        /// <remarks>
        /// Every discovered command class is registered, including one that turns out to be malformed. The resulting
        /// <see cref="InvalidOperationException"/> from <see cref="ConsoleCommandBase"/> then surfaces when the handler is
        /// resolved, which reports the offending class by name rather than silently omitting it from the prompt.
        /// </remarks>
        ///
        /// <author>Almighty-Shogun</author>
        /// <since>1.1.0</since>
        public IServiceCollection RegisterConsoleCommands(Assembly[] assemblies)
            => serviceCollection.RegisterOnInherit<IConsoleCommand>(assemblies, ServiceLifetime.Transient);
    }
}
