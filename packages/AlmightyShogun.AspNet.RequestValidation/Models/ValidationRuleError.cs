namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// One field's failure as the client receives it: a numeric code, a stable name, and a sentence in the negotiated language.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed record ValidationRuleError
{
    /// <summary>
    /// The stable numeric code, derived from the message key so it does not shift when rules are added.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required long Code { get; init; }

    /// <summary>
    /// The machine-readable name a client branches on, which is the part to match rather than the description.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required string Error { get; init; }

    /// <summary>
    /// The human sentence, already resolved into the request's language, and therefore not something to match on.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public required string? ErrorDescription { get; init; }
}
