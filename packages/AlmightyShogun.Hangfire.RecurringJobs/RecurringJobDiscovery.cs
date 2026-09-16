using Cronos;
using Hangfire.Common;
using System.Reflection;
using AlmightyShogun.Utils;
using System.Collections.Immutable;

namespace AlmightyShogun.Hangfire.RecurringJobs;

/// <summary>
/// Discovers and validates recurring jobs.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.2.0</since>
internal static class RecurringJobDiscovery
{
    /// <summary>
    /// Discovers recurring jobs and applies their configuration overrides.
    /// </summary>
    ///
    /// <param name="assemblies">The assemblies to scan.</param>
    /// <param name="settings">The recurring job settings.</param>
    ///
    /// <returns>The active recurring jobs and the ids of disabled jobs.</returns>
    ///
    /// <exception cref="InvalidOperationException">
    /// A job has invalid metadata or execution method, job ids are duplicated,
    /// an override targets an unknown job, or an override contains conflicting values.
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
                ResolveRunMethod(type);
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
    /// Creates the Hangfire invocation for a recurring job.
    /// </summary>
    ///
    /// <param name="job">The recurring job.</param>
    ///
    /// <returns>The Hangfire job invocation.</returns>
    ///
    /// <exception cref="InvalidOperationException">The job type does not expose a public <see cref="IRecurringJob.RunAsync"/> method.</exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    internal static Job CreateExecutionMethod(RecurringJobInfo job)
    {
        MethodInfo runMethod = ResolveRunMethod(job.JobType);

        return new Job(job.JobType, runMethod, [CancellationToken.None], job.Queue);
    }

    /// <summary>
    /// Validates the cron expression, time zone, and queue.
    /// </summary>
    ///
    /// <param name="type">The job type being validated.</param>
    /// <param name="job">The recurring job metadata.</param>
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


    /// <summary>
    /// Determines whether a queue name is valid.
    /// </summary>
    ///
    /// <param name="queue">The queue name to validate.</param>
    ///
    /// <returns><c>true</c> when the queue name is valid; otherwise, <c>false</c>.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
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
    /// The cron expression is invalid.
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
    /// Validates a recurring job override.
    /// </summary>
    ///
    /// <param name="jobId">The recurring job id.</param>
    /// <param name="jobOverride">The override to validate.</param>
    ///
    /// <exception cref="InvalidOperationException">
    /// The override both sets and clears the same value.
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
    /// Resolves the public run method for a recurring job type.
    /// </summary>
    ///
    /// <param name="type">The recurring job type.</param>
    ///
    /// <returns>The run method.</returns>
    ///
    /// <exception cref="InvalidOperationException">
    /// The type does not expose a public <see cref="IRecurringJob.RunAsync"/> method.
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
