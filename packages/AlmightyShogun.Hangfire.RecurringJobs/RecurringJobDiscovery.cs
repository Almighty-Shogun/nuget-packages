using Cronos;
using Hangfire.Common;
using System.Reflection;
using AlmightyShogun.Core;
using System.Collections.Immutable;

namespace AlmightyShogun.Hangfire.RecurringJobs;

/// <summary>
/// Discovers recurring job types, merges the configuration section over what each one declares, and turns the result into
/// scheduling metadata.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.2.0</since>
internal static class RecurringJobDiscovery
{
    /// <summary>
    /// Builds the scheduling metadata for every job type in the provided assemblies, resolving each value from the override
    /// first and the attribute second, and dropping the ones that end up disabled.
    /// </summary>
    ///
    /// <param name="assemblies">
    /// The assemblies to scan. An <see cref="IRecurringJob"/> implementation without the attribute is passed over silently,
    /// since a job invoked directly by other code is a legitimate reason to implement it.
    /// </param>
    /// <param name="settings">
    /// The bound configuration section, applied on top of what each attribute declares. Pass the defaults when the
    /// application has no section, which leaves every job exactly as its attribute states it.
    /// </param>
    ///
    /// <returns>The recurring jobs to schedule, in the order the scan found them.</returns>
    ///
    /// <exception cref="InvalidOperationException">
    /// A job declares or is overridden with an invalid argument, two jobs share a job id, or an override names a job id the
    /// scan did not find. A disabled job is checked and claims its id like any other, so a collision cannot lie dormant
    /// until someone enables it.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>3.0.0</since>
    internal static IReadOnlyList<RecurringJobInfo> GetRecurringJobs(ImmutableArray<Assembly> assemblies, RecurringJobSettings settings)
    {
        List<RecurringJobInfo> jobs = [];
        Dictionary<string, Type> seenJobIds = new(StringComparer.OrdinalIgnoreCase);
        Dictionary<string, RecurringJobOverride> overrides = new(settings.Jobs, StringComparer.OrdinalIgnoreCase);

        foreach (Type type in TypeDiscovery.FindAssignableTypes<IRecurringJob>([.. assemblies]))
        {
            var attribute = type.GetCustomAttribute<RecurringJobAttribute>();

            if (attribute is null) continue;

            RecurringJobOverride? jobOverride = overrides.GetValueOrDefault(attribute.JobId);

            RecurringJobInfo job = new(
                attribute.JobId,
                jobOverride?.CronExpression ?? attribute.CronExpression,
                type,
                jobOverride?.TimeZone ?? attribute.TimeZone,
                jobOverride?.Queue ?? attribute.Queue
            );

            Validate(type, job);

            if (seenJobIds.TryGetValue(job.JobId, out Type? existing))
                throw new InvalidOperationException(
                    $"Recurring job id '{job.JobId}' is declared by both {existing.FullName} and {type.FullName}."
                );

            seenJobIds[job.JobId] = type;

            if (jobOverride?.Enabled ?? attribute.DeclaredEnabled ?? settings.EnabledByDefault)
                jobs.Add(job);
        }

        foreach (string jobId in overrides.Keys.Where(jobId => !seenJobIds.ContainsKey(jobId)))
            throw new InvalidOperationException($"The RecurringJobs configuration overrides '{jobId}', which no discovered job declares.");

        return jobs;
    }

    /// <summary>
    /// Builds the Hangfire invocation for one discovered job, which is the only part of the metadata that would otherwise
    /// have to be carried alongside the public description.
    /// </summary>
    ///
    /// <param name="job">The discovered job to build an invocation for.</param>
    ///
    /// <returns>The Hangfire job that invokes the recurring job implementation.</returns>
    ///
    /// <exception cref="InvalidOperationException">The job type exposes no public run method taking a cancellation token.</exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal static Job CreateExecutionMethod(RecurringJobInfo job)
    {
        MethodInfo runMethod = ResolveRunMethod(job.JobType);

        return new Job(job.JobType, runMethod, [CancellationToken.None], job.Queue);
    }

    /// <summary>
    /// Checks the merged arguments up front, so a mistake stops the host with a message naming the offending type rather
    /// than surfacing as a job that silently never fires.
    /// </summary>
    ///
    /// <param name="type">The job type being validated.</param>
    /// <param name="job">The merged values, so an override is held to the same standard as a declared argument.</param>
    ///
    /// <exception cref="InvalidOperationException">A merged value is not usable.</exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static void Validate(Type type, RecurringJobInfo job)
    {
        if (string.IsNullOrWhiteSpace(job.JobId))
            throw new InvalidOperationException($"{type.FullName} declares an empty recurring job id.");

        if (string.IsNullOrWhiteSpace(job.CronExpression))
            throw new InvalidOperationException($"{type.FullName} resolves to an empty cron expression.");

        try
        {
            CronExpression.Parse(job.CronExpression);
        }
        catch (CronFormatException exception)
        {
            throw new InvalidOperationException(
                $"{type.FullName} resolves to the cron expression '{job.CronExpression}', which is not valid: {exception.Message}",
                exception
            );
        }

        if (job.TimeZone is null) return;

        try
        {
            TimeZoneInfo.FindSystemTimeZoneById(job.TimeZone);
        }
        catch (Exception exception) when (exception is TimeZoneNotFoundException or InvalidTimeZoneException)
        {
            throw new InvalidOperationException(
                $"{type.FullName} resolves to the time zone '{job.TimeZone}', which this system does not recognise.",
                exception
            );
        }
    }

    /// <summary>
    /// Finds the implementation Hangfire invokes, rather than the interface declaration, so the serialized job records the
    /// concrete type's method.
    /// </summary>
    ///
    /// <param name="type">The job type.</param>
    ///
    /// <returns>The method Hangfire should invoke.</returns>
    ///
    /// <exception cref="InvalidOperationException">
    /// The type exposes no public matching run method, which happens when the interface is implemented explicitly, since an
    /// explicit implementation is private and Hangfire cannot invoke it.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static MethodInfo ResolveRunMethod(Type type)
    {
        const string runAsync = nameof(IRecurringJob.RunAsync);

        MethodInfo? method = type.GetMethod(
            runAsync,
            BindingFlags.Public | BindingFlags.Instance,
            null,
            [typeof(CancellationToken)],
            null
        );

        return method ?? throw new InvalidOperationException(
            $"{type.FullName} does not expose a public {runAsync} method. Implement {nameof(IRecurringJob)} publicly, not explicitly."
        );
    }
}
