using System.Reflection;
using System.Collections.Immutable;

namespace AlmightyShogun.Hangfire.RecurringJobs;

/// <summary>
/// Carries the assemblies the attribute scan runs over, so the scan is a container-resolved service rather than work done
/// inside the registration call.
/// </summary>
///
/// <param name="Assemblies">
/// The assemblies to scan for recurring job classes. Immutable because the scan runs when the host starts rather than when
/// this is constructed, which would otherwise leave the caller's array editable in between.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed record RecurringJobSources(ImmutableArray<Assembly> Assemblies);
