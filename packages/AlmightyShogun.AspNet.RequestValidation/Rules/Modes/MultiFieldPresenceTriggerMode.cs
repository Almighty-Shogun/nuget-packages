namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// What makes a multi-field rule fire: any of the named fields, or all of them, being present or missing.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal enum MultiFieldPresenceTriggerMode
{
    /// <summary>
    /// Triggers when any related field is present.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    WithAny,

    /// <summary>
    /// Triggers when all related fields are present.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    WithAll,

    /// <summary>
    /// Triggers when any related field is missing.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    WithoutAny,

    /// <summary>
    /// Triggers when all related fields are missing.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    WithoutAll,

    /// <summary>
    /// Triggers prohibit behavior when related fields are present.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Prohibits
}
