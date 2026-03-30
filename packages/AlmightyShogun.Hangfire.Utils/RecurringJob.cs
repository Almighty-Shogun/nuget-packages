using Hangfire.Common;

namespace AlmightyShogun.Hangfire.Utils;

public class RecurringJob(string jobId, string cronExpression, Job executionMethod)
{
    public string Name => jobId;
    public string CronExpression => cronExpression;
    public Job Method => executionMethod;
}
