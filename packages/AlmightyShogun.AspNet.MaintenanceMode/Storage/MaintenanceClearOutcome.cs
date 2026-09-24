namespace AlmightyShogun.AspNet.MaintenanceMode;

/// <summary>
/// Describes the result of a revision-checked maintenance clear.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal enum MaintenanceClearOutcome
{
    /// <summary>
    /// The matching window was deleted.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Cleared,

    /// <summary>
    /// The expected window is no longer current. The cache reflects the observed state.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Superseded,

    /// <summary>
    /// The file could not be verified. Nothing was deleted or published.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Unverified
}
