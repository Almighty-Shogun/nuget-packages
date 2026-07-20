namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Defines the format options used by validation rules.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal enum FormatMode
{
    /// <summary>
    /// Requires a valid email address format.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Email,

    /// <summary>
    /// Requires a valid URL format.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Url,

    /// <summary>
    /// Requires a valid JSON format.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Json,

    /// <summary>
    /// Requires a valid UUID format.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Uuid,

    /// <summary>
    /// Requires a valid ULID format.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Ulid,

    /// <summary>
    /// Requires a valid hexadecimal color format.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    HexColor,

    /// <summary>
    /// Requires a valid MAC address format.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    MacAddress
}
