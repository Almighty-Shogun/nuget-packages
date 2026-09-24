namespace AlmightyShogun.AspNet.MaintenanceMode;

/// <summary>
/// Describes the result of reading the persisted maintenance state.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal enum DiskReadOutcome
{
    /// <summary>
    /// No state file exists.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Missing,

    /// <summary>
    /// The file was read successfully and its state is authoritative.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Loaded,

    /// <summary>
    /// The file is invalid and a fail-closed state was substituted.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Corrupt,

    /// <summary>
    /// The file could not be read, so its current state is unknown.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Unreadable
}
