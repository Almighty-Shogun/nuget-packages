namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// The rule family a validation attribute belongs to. It is the discriminator that decides how the loosely typed mode stored beside it is
/// cast back, so the two are always assigned together in one base constructor rather than set independently.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal enum AttributeRuleType
{
    /// <summary>
    /// Whether the field must exist, hold a value, be absent, or be forbidden. Checked before any family that inspects a value.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Presence,

    /// <summary>
    /// Whether the bound value is the shape the rule names, such as a number, a boolean, or a collection.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Type,

    /// <summary>
    /// Whether text matches a known shape such as an email address, a URL, a UUID, or a colour.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Format,

    /// <summary>
    /// Whether text is an IP address, either family or one of them specifically.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Ip,

    /// <summary>
    /// Which characters the text may contain, or the case it must already be written in.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    StringCharacter,

    /// <summary>
    /// Whether the text contains, starts with, or ends with one of a set of values.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    StringMatch,

    /// <summary>
    /// The negation of the matching family. It shares that family's mode set, so the two differ only in the result they report.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    DoesNot,

    /// <summary>
    /// A magnitude comparison, where the measured quantity depends on the bound type: a number's value, a string's length, a collection's
    /// count, or a file's size.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    ComparableSize,

    /// <summary>
    /// A count of the digits written in the value, rather than a comparison of the number it spells.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Digit,

    /// <summary>
    /// A restriction on an upload: its extension, its MIME type, or the dimensions of the image it holds.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    File,

    /// <summary>
    /// A comparison against another field on the same request, such as a confirmation pairing or a must-differ pairing.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    FieldComparison,

    /// <summary>
    /// A rule that only applies when a controlling field equals one of a set of values.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    Conditional,

    /// <summary>
    /// A rule that only applies when a controlling field reads as accepted or declined, which is truthiness rather than equality.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    ConditionalState,

    /// <summary>
    /// A rule keyed on whether several other fields are present or missing, applying on any of them or on all of them.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    MultiFieldPresence,

    /// <summary>
    /// A date comparison against a literal date or against another property, which is why the target's kind travels with it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    DateComparison
}
