namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// The band a rule runs in. Only two exist because the single ordering that matters is presence before everything else; finer ordering is
/// the declaration order within a band.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal enum ValidationRulePriority
{
    /// <summary>
    /// The early band, claimed by presence rules so an absent field is reported as absent rather than as the wrong shape.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Required,

    /// <summary>
    /// The default band, where rules run in the order they were declared.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Normal
}
