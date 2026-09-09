using System.Reflection;
using System.Collections.Immutable;

namespace AlmightyShogun.Hangfire.RecurringJobs;

/// <summary>
/// Carries the assemblies one registration call named, so the scan is a container-resolved service rather than work done
/// inside that call. Each call registers its own instance, and <see cref="RecurringJobRegistry"/> scans all of them.
/// </summary>
///
/// <param name="Assemblies">
/// The assemblies this call named, scanned for recurring job classes. Immutable because the scan runs when the host starts
/// rather than when this is constructed.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
internal sealed record RecurringJobSources(ImmutableArray<Assembly> Assemblies);
