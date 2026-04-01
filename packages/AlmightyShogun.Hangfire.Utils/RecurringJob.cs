using Hangfire.Common;

namespace AlmightyShogun.Hangfire.Utils;

public record RecurringJob(string Name, string CronExpression, Job ExecutionMethod);
