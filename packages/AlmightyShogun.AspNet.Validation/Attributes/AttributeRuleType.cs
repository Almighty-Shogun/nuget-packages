namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Defines the validation rule category created by a validation attribute.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal enum AttributeRuleType
{
    /// <summary>
    /// Creates a presence validation rule.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Presence,

    /// <summary>
    /// Creates a type validation rule.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Type,

    /// <summary>
    /// Creates a format validation rule.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Format,

    /// <summary>
    /// Creates an IP address validation rule.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Ip,

    /// <summary>
    /// Creates a string character validation rule.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    StringCharacter,

    /// <summary>
    /// Creates a string matching validation rule.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    StringMatch,

    /// <summary>
    /// Creates an inverse string matching validation rule.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    DoesNot,

    /// <summary>
    /// Creates a comparable size validation rule.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    ComparableSize,

    /// <summary>
    /// Creates a digit count validation rule.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Digit,

    /// <summary>
    /// Creates a file constraint validation rule.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    File,

    /// <summary>
    /// Creates a field comparison validation rule.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    FieldComparison,

    /// <summary>
    /// Creates a conditional validation rule.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Conditional,

    /// <summary>
    /// Creates a conditional state validation rule.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    ConditionalState,

    /// <summary>
    /// Creates a multi-field presence validation rule.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    MultiFieldPresence,

    /// <summary>
    /// Creates a date comparison validation rule.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    DateComparison
}
