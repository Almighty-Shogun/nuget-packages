using Cronos;
using Hangfire.Common;
using System.Reflection;
using AlmightyShogun.Utils;
using System.Collections.Immutable;

namespace AlmightyShogun.Hangfire.RecurringJobs;

/// <summary>
/// Overrides recurring job settings for a specific environment.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.2.0</since>
internal static class RecurringJobDiscovery
{
    /// <summary>
    /// Builds the scheduling metadata for every job type in the provided assemblies, taking the job id from the attribute,
    /// the cron expression, time zone and queue from the override first and the attribute second, and enablement from the
    /// override, then the attribute, then the section's default. A job that ends up disabled contributes its id rather than
    /// its metadata, since those are the ids <see cref="JobSchedulerStartup"/> unschedules when
    /// <see cref="RecurringJobSettings.RemoveParkedJobs"/> is set.
    /// </summary>
    ///
    /// <param name="assemblies">
    /// The assemblies to scan. An <see cref="IRecurringJob"/> implementation without the attribute is passed over silently,
    /// since a job invoked directly by other code is a legitimate reason to implement it.
    /// </param>
    /// <param name="settings">The configuration section. Pass the defaults when the application has no section.</param>
    ///
    /// <returns>
    /// The recurring jobs to schedule, in the order the scan found them, alongside the ids of the ones that resolved to
    /// disabled.
    /// </returns>
    ///
    /// <exception cref="InvalidOperationException">
    /// A job declares invalid scheduling metadata, two jobs share a job id, an override names an unknown job,
    /// or an override both sets and clears the same value.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>3.0.0</since>
    internal static RecurringJobScan GetRecurringJobs(ImmutableArray<Assembly> assemblies, RecurringJobSettings settings)
    {
        List<string> parkedJobIds = [];
        List<RecurringJobInfo> jobs = [];

        Dictionary<string, Type> seenJobIds = new(StringComparer.OrdinalIgnoreCase);
        Dictionary<string, RecurringJobOverride> overrides = new(settings.Jobs, StringComparer.OrdinalIgnoreCase);

        foreach (Type type in TypeDiscovery.FindAssignableTypes<IRecurringJob>([.. assemblies]))
        {
            var attribute = type.GetCustomAttribute<RecurringJobAttribute>();

            if (attribute is null) continue;

            string jobId = attribute.JobId;

            if (string.IsNullOrWhiteSpace(jobId))
                throw new InvalidOperationException($"{type.FullName} declares an empty recurring job id.");

            RecurringJobOverride? jobOverride = overrides.GetValueOrDefault(jobId);

            if (jobOverride is not null)
                ValidateJobOverride(jobId, jobOverride);

            RecurringJobInfo job = new()
            {
                JobId = jobId,
                CronExpression = jobOverride?.CronExpression ?? attribute.CronExpression,
                JobType = type,
                TimeZone = jobOverride switch
                {
                    { ClearTimeZone: true } => null,
                    { TimeZone: not null } => jobOverride.TimeZone,
                    _ => attribute.TimeZone
                },
                Queue = jobOverride switch
                {
                    { ClearQueue: true } => null,
                    { Queue: not null } => jobOverride.Queue,
                    _ => attribute.Queue
                }
            };

            if (seenJobIds.TryGetValue(jobId, out Type? existing))
                throw new InvalidOperationException(
                    $"Recurring job id '{jobId}' is declared by both {existing.FullName} and {type.FullName}.");

            seenJobIds[jobId] = type;

            bool enabled = jobOverride?.Enabled ?? attribute.DeclaredEnabled ?? settings.EnabledByDefault;

            if (enabled)
            {
                Validate(type, job);
                jobs.Add(job);
            }
            else
            {
                parkedJobIds.Add(jobId);
            }
        }

        foreach (string jobId in overrides.Keys.Where(jobId => !seenJobIds.ContainsKey(jobId)))
            throw new InvalidOperationException($"The RecurringJobs configuration overrides '{jobId}', which no discovered job declares.");

        return new RecurringJobScan(jobs, parkedJobIds);
    }

    /// <summary>
    /// Builds the Hangfire invocation for one discovered job.
    /// </summary>
    ///
    /// <param name="job">The discovered job to build an invocation for.</param>
    ///
    /// <returns>The Hangfire job that invokes the recurring job implementation.</returns>
    ///
    /// <exception cref="InvalidOperationException">The job type exposes no public run method taking a cancellation token.</exception>
    ///
    /// <remarks>
    /// The invocation pins <c>CancellationToken.None</c> as the argument. Hangfire substitutes the running server's own
    /// token for a cancellation token argument before it invokes the method, so this stands in for a token rather than
    /// being the one the job receives.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    internal static Job CreateExecutionMethod(RecurringJobInfo job)
    {
        MethodInfo runMethod = ResolveRunMethod(job.JobType);

        return new Job(job.JobType, runMethod, [CancellationToken.None], job.Queue);
    }

    /// <summary>
    /// Validates the merged cron expression, time zone, and queue.
    /// </summary>
    ///
    /// <param name="type">The job type being validated.</param>
    /// <param name="job">The merged recurring job metadata.</param>
    ///
    /// <exception cref="InvalidOperationException">
    /// The cron expression, time zone, or queue is invalid.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static void Validate(Type type, RecurringJobInfo job)
    {
        if (string.IsNullOrWhiteSpace(job.CronExpression))
            throw new InvalidOperationException($"{type.FullName} resolves to an empty cron expression.");

        try
        {
            ValidateCron(job.CronExpression);
        }
        catch (CronFormatException exception)
        {
            throw new InvalidOperationException(
                $"{type.FullName} resolves to the cron expression '{job.CronExpression}', which is not valid: {exception.Message}",
                exception);
        }

        if (job.TimeZone is not null)
        {
            try
            {
                TimeZoneInfo.FindSystemTimeZoneById(job.TimeZone);
            }
            catch (Exception exception) when (exception is TimeZoneNotFoundException or InvalidTimeZoneException)
            {
                throw new InvalidOperationException(
                    $"{type.FullName} resolves to the time zone '{job.TimeZone}', which this system does not recognise.",
                    exception);
            }
        }

        if (job.Queue is not null && !IsValidQueue(job.Queue))
            throw new InvalidOperationException(
                $"{type.FullName} resolves to the queue '{job.Queue}', which is not a valid Hangfire queue name.");
    }


    private static bool IsValidQueue(string queue)
    {
        if (string.IsNullOrWhiteSpace(queue))
            return false;

        return queue.All(character =>
            character is >= 'a' and <= 'z'
                or >= '0' and <= '9'
                or '_' or '-');
    }

    /// <summary>
    /// Validates a five- or six-field cron expression.
    /// </summary>
    ///
    /// <param name="cronExpression">The cron expression to validate.</param>
    ///
    /// <exception cref="CronFormatException">
    /// The expression is not a valid five- or six-field cron expression.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static void ValidateCron(string cronExpression)
    {
        try
        {
            CronExpression.Parse(cronExpression);
        }
        catch (CronFormatException)
        {
            CronExpression.Parse(cronExpression, CronFormat.IncludeSeconds);
        }
    }

    /// <summary>
    /// Validates mutually exclusive recurring job override values.
    /// </summary>
    ///
    /// <param name="jobId">The recurring job id.</param>
    /// <param name="jobOverride">The override to validate.</param>
    ///
    /// <exception cref="InvalidOperationException">
    /// A value is specified together with its corresponding clear option.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static void ValidateJobOverride(string jobId, RecurringJobOverride jobOverride)
    {
        switch (jobOverride)
        {
            case { ClearTimeZone: true, TimeZone: not null }:
                throw new InvalidOperationException($"Recurring job '{jobId}' cannot specify both a time zone and ClearTimeZone.");
            case { ClearQueue: true, Queue: not null }:
                throw new InvalidOperationException($"Recurring job '{jobId}' cannot specify both a queue and ClearQueue.");
        }
    }

    /// <summary>
    /// Finds the implementation Hangfire invokes, rather than the interface declaration, so the serialized job records the
    /// concrete type's method.
    /// </summary>
    ///
    /// <param name="type">The job class, whose own public declaration is searched rather than the interface's.</param>
    ///
    /// <returns>The method Hangfire should invoke.</returns>
    ///
    /// <exception cref="InvalidOperationException">
    /// The type exposes no public matching run method, which happens when the interface is implemented explicitly, since an
    /// explicit implementation is private and Hangfire cannot invoke it.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    private static MethodInfo ResolveRunMethod(Type type)
    {
        const string runAsync = nameof(IRecurringJob.RunAsync);

        MethodInfo? method = type.GetMethod(
            runAsync,
            BindingFlags.Public | BindingFlags.Instance,
            null,
            [typeof(CancellationToken)],
            null);

        return method ?? throw new InvalidOperationException(
            $"{type.FullName} does not expose a public {runAsync} method. Implement {nameof(IRecurringJob)} publicly, not explicitly.");
    }
}
