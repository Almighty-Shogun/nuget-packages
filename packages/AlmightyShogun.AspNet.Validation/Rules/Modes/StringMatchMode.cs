namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Defines the string match options used by validation rules.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal enum StringMatchMode
{
    /// <summary>
    /// Checks whether text contains a configured value.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Contain,

    /// <summary>
    /// Checks whether text ends with a configured value.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    EndWith,

    /// <summary>
    /// Checks whether text starts with a configured value.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    StartWith
}
