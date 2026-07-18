using Hangfire.Common;

namespace AlmightyShogun.Hangfire.Utils;

/// <summary>
/// Stores the Hangfire metadata required to schedule one discovered recurring job.
/// </summary>
///
/// <param name="Name">The stable recurring job id.</param>
/// <param name="CronExpression">The cron expression used for the recurring schedule.</param>
/// <param name="ExecutionMethod">The Hangfire job that invokes the recurring job implementation.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>2.2.0</since>
internal sealed record RecurringJob(string Name, string CronExpression, Job ExecutionMethod);
