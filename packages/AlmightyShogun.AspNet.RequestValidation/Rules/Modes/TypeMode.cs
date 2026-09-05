namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Which shape the bound value must have. An absent or empty value satisfies <c>Array</c>, <c>List</c> and <c>File</c>, which
/// short-circuit on it; <c>String</c> and <c>Boolean</c> apply their own narrower notion instead. None implies the field is
/// required.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal enum TypeMode
{
    /// <summary>
    /// Text, or nothing. A number bound to an object property is not text and fails.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    String,

    /// <summary>
    /// A boolean, or text that parses as one, so a form posting <c>true</c> as a string still passes.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Boolean,

    /// <summary>
    /// A sequence that is not a string, since a string is enumerable but is never meant as a collection here.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Array,

    /// <summary>
    /// Requires an array, a non-generic list, or a value implementing a generic list interface.
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
