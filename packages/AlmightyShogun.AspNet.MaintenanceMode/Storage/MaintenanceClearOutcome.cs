namespace AlmightyShogun.AspNet.MaintenanceMode;

/// <summary>
/// How a conditional clear ended. The two refusals differ in what the caller can do about them: one leaves the store holding a value the
/// caller has not seen yet, the other leaves the file and the cache exactly as they were.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal enum MaintenanceClearOutcome
{
    /// <summary>
    /// The file was read, held the revision the caller expected, and was deleted, so the window the caller acted on is closed.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Cleared,

    /// <summary>
    /// The store established that the expected parsed window is not present: the file holds a different one, holds none, or could not be
    /// parsed. Nothing was deleted, and the cache now serves what that check produced, so reading again returns something other than the
    /// value the caller acted on.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Superseded,

    /// <summary>
    /// The file's contents could not be established, so no revision was compared and nothing was deleted. The cache was left alone, so a
    /// caller that reads again is served the value it already acted on unless something else has retired that entry in the meantime.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Unverified
}
