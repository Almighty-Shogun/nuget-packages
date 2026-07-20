namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Defines the array key options used by validation rules.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal enum ArrayKeyMode
{
    /// <summary>
    /// Requires at least one configured key to exist.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    AnyRequiredKey,

    /// <summary>
    /// Requires every configured key to exist.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    AllRequiredKeys
}
