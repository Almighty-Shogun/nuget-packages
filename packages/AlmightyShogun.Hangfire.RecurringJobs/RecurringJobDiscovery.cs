using Cronos;
using Hangfire.Common;
using System.Reflection;
using AlmightyShogun.Core;

namespace AlmightyShogun.Hangfire.RecurringJobs;

/// <summary>
/// Discovers recurring job types and turns them into scheduling metadata.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>2.2.0</since>
internal static class RecurringJobDiscovery
{
    /// <summary>
    /// Builds the recurring job metadata for every job type declared in the provided assemblies.
    /// </summary>
    ///
    /// <param name="assemblies">
    /// The assemblies to scan. A <see cref="RecurringJobBase"/> subclass without the attribute is passed over silently, since
    /// an abstract or helper base is a legitimate reason to have one.
    /// </param>
    ///
    /// <returns>The recurring jobs to schedule, in the order the scan found them.</returns>
    ///
    /// <exception cref="InvalidOperationException">
    /// A job declares an invalid attribute argument, or two jobs share a job id.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>3.0.0</since>
    internal static IReadOnlyList<RecurringJobInfo> GetRecurringJobs(Assembly[] assemblies)
    {
        List<RecurringJobInfo> jobs = [];
        Dictionary<string, Type> seenJobIds = new(StringComparer.OrdinalIgnoreCase);

        foreach (Type type in TypeDiscovery.FindAssignableTypes<RecurringJobBase>(assemblies))
        {
            var attribute = type.GetCustomAttribute<RecurringJobAttribute>();

            if (attribute is null) continue;

            Validate(type, attribute);

            if (!attribute.Enabled) continue;

            if (seenJobIds.TryGetValue(attribute.JobId, out Type? existing))
                throw new InvalidOperationException(
                    $"Recurring job id '{attribute.JobId}' is declared by both {existing.FullName} and {type.FullName}."
                );

            seenJobIds[attribute.JobId] = type;

            jobs.Add(new RecurringJobInfo(
                attribute.JobId,
                attribute.CronExpression,
                type,
                attribute.TimeZone,
                attribute.Queue
            ));
        }

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
    /// Checks the attribute arguments up front, so a mistake stops the host with a message naming the offending type
    /// rather than surfacing as a job that silently never fires.
    /// </summary>
    ///
    /// <param name="type">The job type being validated.</param>
    /// <param name="attribute">The attribute declared on it.</param>
    ///
    /// <exception cref="InvalidOperationException">An attribute argument is not usable.</exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static void Validate(Type type, RecurringJobAttribute attribute)
    {
        if (string.IsNullOrWhiteSpace(attribute.JobId))
            throw new InvalidOperationException($"{type.FullName} declares an empty recurring job id.");

        if (string.IsNullOrWhiteSpace(attribute.CronExpression))
            throw new InvalidOperationException($"{type.FullName} declares an empty cron expression.");

        try
        {
            CronExpression.Parse(attribute.CronExpression);
        }
        catch (CronFormatException exception)
        {
            throw new InvalidOperationException(
                $"{type.FullName} declares the cron expression '{attribute.CronExpression}', which is not valid: {exception.Message}",
                exception
            );
        }

        if (attribute.TimeZone is null) return;

        try
        {
            TimeZoneInfo.FindSystemTimeZoneById(attribute.TimeZone);
        }
        catch (Exception exception) when (exception is TimeZoneNotFoundException or InvalidTimeZoneException)
        {
            throw new InvalidOperationException(
                $"{type.FullName} declares the time zone '{attribute.TimeZone}', which this system does not recognise.",
                exception
            );
        }
    }

    /// <summary>
    /// Finds the override Hangfire invokes, rather than the abstract declaration, so the serialized job records the concrete
    /// type's method.
    /// </summary>
    ///
    /// <param name="type">The job type.</param>
    ///
    /// <returns>The method Hangfire should invoke.</returns>
    ///
    /// <exception cref="InvalidOperationException">
    /// The type exposes no matching run method, which a concrete subclass cannot normally manage since the base class
    /// declares one as abstract.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static MethodInfo ResolveRunMethod(Type type)
    {
        const string runAsync = nameof(RecurringJobBase.RunAsync);

        MethodInfo? method = type.GetMethod(
            runAsync,
            BindingFlags.Public | BindingFlags.Instance,
            null,
            [typeof(CancellationToken)],
            null
        );

        return method ?? throw new InvalidOperationException($"{type.FullName} does not expose a public {runAsync} method.");
    }
}
