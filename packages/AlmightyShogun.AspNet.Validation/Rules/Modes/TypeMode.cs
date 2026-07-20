namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Defines the type options used by validation rules.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal enum TypeMode
{
    /// <summary>
    /// Requires a string-compatible value.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    String,

    /// <summary>
    /// Requires a boolean-compatible value.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Boolean,

    /// <summary>
    /// Requires an array-compatible value.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Array,

    /// <summary>
    /// Requires a generic list-compatible value.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    List,

    /// <summary>
    /// Requires an uploaded file-compatible value.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    File
}
