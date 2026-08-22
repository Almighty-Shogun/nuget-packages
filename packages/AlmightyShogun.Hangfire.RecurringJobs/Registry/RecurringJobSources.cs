using System.Reflection;

namespace AlmightyShogun.Hangfire.RecurringJobs;

/// <summary>
/// Carries the assemblies the attribute scan runs over, so the scan is a container-resolved service rather than work done
/// inside the registration call.
/// </summary>
///
/// <param name="Assemblies">The assemblies to scan for recurring job classes.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed record RecurringJobSources(Assembly[] Assemblies);
