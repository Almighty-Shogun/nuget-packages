namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Which presence condition applies. Required, filled, and present differ in how they treat a field that was posted but left empty, which
/// is the distinction most easily got wrong.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal enum PresenceMode
{
    /// <summary>
    /// The field must be present and hold something. The strictest of the three, and the one that fails a field posted blank.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Required,

    /// <summary>
    /// The field may be omitted, but if it is sent it must hold something. Use it for an optional value that must not be blanked.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Filled,

    /// <summary>
    /// The field must be sent, but may be sent empty. Use it when the client's silence and its blank are different answers.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Present,

    /// <summary>
    /// The field must not be sent at all. A field sent empty still fails, because it was sent.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Missing,

    /// <summary>
    /// The field must carry nothing, whether by being absent or by being sent empty. The looser counterpart of missing.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Prohibited
}
