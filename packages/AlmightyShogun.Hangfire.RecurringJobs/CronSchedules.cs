namespace AlmightyShogun.Hangfire.RecurringJobs;

/// <summary>
/// Common cron expressions, so a schedule can be written without hand-typing one.
/// </summary>
///
/// <remarks>
/// These are ordinary cron strings rather than helper calls, so any other expression Cronos parses can be written out by
/// hand instead. All of them are evaluated in UTC unless the job sets <see cref="RecurringJobAttribute.TimeZone"/> or the
/// configuration section overrides it.
/// </remarks>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public static class CronSchedules
{
    /// <summary>
    /// Runs once a minute.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public const string Minutely = "* * * * *";

    /// <summary>
    /// Runs at the start of every hour.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public const string Hourly = "0 * * * *";

    /// <summary>
    /// Runs every day at midnight.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public const string Daily = "0 0 * * *";

    /// <summary>
    /// Runs every Monday at midnight.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public const string Weekly = "0 0 * * 1";

    /// <summary>
    /// Runs on the first day of every month at midnight.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public const string Monthly = "0 0 1 * *";

    /// <summary>
    /// Runs on the first day of every year at midnight.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public const string Yearly = "0 0 1 1 *";
}
