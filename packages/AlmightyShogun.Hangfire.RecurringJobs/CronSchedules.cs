namespace AlmightyShogun.Hangfire.RecurringJobs;

/// <summary>
/// Common cron expressions.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>4.0.0</since>
public static class CronSchedules
{
    /// <summary>
    /// Runs once a minute.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public const string Minutely = "* * * * *";

    /// <summary>
    /// Runs at the start of every hour.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public const string Hourly = "0 * * * *";

    /// <summary>
    /// Runs every day at midnight.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public const string Daily = "0 0 * * *";

    /// <summary>
    /// Runs every Monday at midnight.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public const string Weekly = "0 0 * * 1";

    /// <summary>
    /// Runs on the first day of every month at midnight.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public const string Monthly = "0 0 1 * *";

    /// <summary>
    /// Runs on the first day of every year at midnight.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>4.0.0</since>
    public const string Yearly = "0 0 1 1 *";
}
