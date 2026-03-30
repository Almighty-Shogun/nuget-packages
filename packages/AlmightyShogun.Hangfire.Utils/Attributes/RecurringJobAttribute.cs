namespace AlmightyShogun.Hangfire.Utils.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class RecurringJobAttribute(string jobId, string cronExpression) : Attribute
{
    public string JobId { get; } = jobId;
    public string CronExpression { get; } = cronExpression;
}
