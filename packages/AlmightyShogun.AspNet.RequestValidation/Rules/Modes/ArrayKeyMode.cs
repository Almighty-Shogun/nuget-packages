namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Whether a keyed field must carry any of the named keys or all of them. One rule serves both by switching on this.
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
