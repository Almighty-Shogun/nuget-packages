namespace AlmightyShogun.AspNet.MaintenanceMode;

/// <summary>
/// What a read of the state file established. Only one of these carries a revision the file is known to hold, which is what a
/// conditional clear has to compare against before it deletes anything.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal enum DiskReadOutcome
{
    /// <summary>
    /// The file is not there, so no window is recorded and there is nothing to serve.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Missing,

    /// <summary>
    /// The file was opened and parsed, so the accompanying window and its revision are the file's own.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Loaded,

    /// <summary>
    /// The file was opened but does not parse, so the accompanying window is the fail-closed one rather than anything the file holds.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Corrupt,

    /// <summary>
    /// The file is there but every attempt to open it failed, so the accompanying value is the last cached one and says nothing about
    /// what the file holds now.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Unreadable
}
