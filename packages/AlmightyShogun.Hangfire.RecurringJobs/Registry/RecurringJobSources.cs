using System.Reflection;
using System.Collections.Immutable;

namespace AlmightyShogun.Hangfire.RecurringJobs;

/// <summary>
/// Contains assemblies to scan for recurring jobs.
/// </summary>
///
/// <param name="Assemblies"> The assemblies to scan.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal sealed record RecurringJobSources(ImmutableArray<Assembly> Assemblies);
