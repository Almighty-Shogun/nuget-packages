using Cronos;
using Hangfire.Common;
using System.Reflection;
using AlmightyShogun.Utils;
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
    /// Builds the scheduling metadata for every job type in the provided assemblies, taking the job id from the attribute,
    /// the cron expression, time zone and queue from the override first and the attribute second, and enablement from the
    /// override, then the attribute, then the section's default. Jobs that end up disabled are dropped.
    /// </summary>
    ///
    /// <param name="assemblies">
    /// The assemblies to scan. An <see cref="IRecurringJob"/> implementation without the attribute is passed over silently,
    /// since a job invoked directly by other code is a legitimate reason to implement it.
    /// </param>
    /// <param name="settings">
    /// The configuration section. Its per-job entries override what an attribute declares, while <c>EnabledByDefault</c> sits
    /// beneath one, standing in only for a job whose attribute never set <c>Enabled</c>. Pass the defaults when the
    /// application has no section.
    /// </param>
    ///
    /// <returns>The recurring jobs to schedule, in the order the scan found them.</returns>
    ///
    /// <exception cref="ArgumentNullException">
    /// A job's attribute declares a <c>null</c> job id, which fails while the per-job overrides are looked up, before the job
    /// id is checked at all.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// A job declares a blank job id, or declares or is overridden with an unparseable cron expression or an unknown time
    /// zone, two jobs share a job id, or an override names a job id the scan did not find. The job id comes from the
    /// attribute alone and cannot be overridden. A disabled job is checked and claims its id like any other, so a collision
    /// cannot lie dormant until someone enables it. The merged queue is not among the checked values.
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

            RecurringJobInfo job = new()
            {
                JobId = attribute.JobId,
                CronExpression = jobOverride?.CronExpression ?? attribute.CronExpression,
                JobType = type,
                TimeZone = jobOverride?.TimeZone ?? attribute.TimeZone,
                Queue = jobOverride?.Queue ?? attribute.Queue
            };

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
    /// Builds the Hangfire invocation for one discovered job.
    /// </summary>
    ///
    /// <param name="job">The discovered job to build an invocation for.</param>
    ///
    /// <returns>The Hangfire job that invokes the recurring job implementation.</returns>
    ///
    /// <exception cref="ArgumentException">
    /// The merged queue name is not one Hangfire accepts. Nothing in this package checks the queue, so the Hangfire job
    /// constructor is what rejects it.
    /// </exception>
    /// <exception cref="InvalidOperationException">The job type exposes no public run method taking a cancellation token.</exception>
    ///
    /// <remarks>
    /// The invocation pins <c>CancellationToken.None</c> as the argument. Hangfire substitutes the running server's own
    /// token for a cancellation token argument before it invokes the method, so this stands in for a token rather than
    /// being the one the job receives.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal static Job CreateExecutionMethod(RecurringJobInfo job)
    {
        MethodInfo runMethod = ResolveRunMethod(job.JobType);

        return new Job(job.JobType, runMethod, [CancellationToken.None], job.Queue);
    }

    /// <summary>
    /// Checks the merged job id, cron expression and time zone up front, so a mistake in one of them stops the host with a
    /// message naming the offending type. Each would stop the host anyway, from the time zone lookup or from Hangfire's own
    /// cron validation, but without naming which job caused it. The merged queue is not
    /// checked, here or anywhere else in the package, so a name Hangfire rejects surfaces from
    /// <see cref="CreateExecutionMethod"/> instead.
    /// </summary>
    ///
    /// <param name="type">The job type being validated.</param>
    /// <param name="job">
    /// The merged values, so an overridden cron expression or time zone is checked exactly like a declared one.
    /// </param>
    ///
    /// <exception cref="InvalidOperationException">The merged job id, cron expression or time zone is not usable.</exception>
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
