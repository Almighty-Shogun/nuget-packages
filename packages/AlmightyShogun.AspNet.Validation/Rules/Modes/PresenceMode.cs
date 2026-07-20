namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Defines the presence options used by validation rules.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal enum PresenceMode
{
    /// <summary>
    /// Requires the value to be non-empty.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Required,

    /// <summary>
    /// Requires the value to be filled when present.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Filled,

    /// <summary>
    /// Requires the field to be present.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Present,

    /// <summary>
    /// Requires the field to be missing.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Missing,

    /// <summary>
    /// Requires the value to be empty or absent.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Prohibited
}
