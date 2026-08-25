using System.Text.RegularExpressions;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Matches text against a caller-supplied pattern, in either direction, under a timeout so a pathological pattern cannot hang a request.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class RegexValidationRule<TRequest, TProperty>
    : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    /// <summary>
    /// The expression, built once when the rule is. Holding the instance keeps it out of the process-wide static cache, which holds fifteen
    /// entries and starts discarding patterns once an application declares more than that.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly Regex _regex;

    /// <summary>
    /// Whether a match means success or failure. One rule class serves both the matching and the not-matching spellings, so this is what
    /// inverts the outcome rather than a second class existing.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly bool _shouldMatch;

    /// <summary>
    /// The optional human-readable description of the expected shape, surfaced as a message parameter.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly string? _description;

    /// <summary>
    /// Builds the expression once, when the rule is built for its request type, so a request never pays to parse the pattern.
    /// </summary>
    ///
    /// <param name="pattern">The pattern to match against. Supplied by the caller, so it is never validated for cost here.</param>
    /// <param name="options">
    /// The options to build with, passed through untouched. Notably <c>Compiled</c> is not added: it trades startup time and native memory
    /// that never reverses for match speed, which only pays off on a pattern that runs often.
    /// </param>
    /// <param name="shouldMatch">Whether matching means success, which is what separates this rule from its negated spelling.</param>
    /// <param name="description">
    /// A human-readable description of the expected shape, substituted into the failure message so a client is told what was wanted rather
    /// than shown the pattern.
    /// </param>
    /// <param name="matchTimeout">
    /// How long one match may run before it is abandoned and the rule fails. Defaults to a second, which bounds a pattern that backtracks
    /// catastrophically on hostile input.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RegexValidationRule(
        string pattern,
        RegexOptions options,
        bool shouldMatch,
        string? description = null,
        TimeSpan? matchTimeout = null
    )
    {
        _regex = new Regex(pattern, options, matchTimeout ?? TimeSpan.FromSeconds(1));

        _shouldMatch = shouldMatch;
        _description = description;
    }

    /// <inheritdoc />
    public ValueTask<ValidationRuleResult> ValidateAsync(
        TRequest request,
        TProperty? value,
        string field,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default
    )
    {
        if (ValidationValue.IsEmpty(value))
            return ValueTask.FromResult(ValidationRuleResult.Success());

        if (!ValidationValue.TryGetText(value, out string text))
            return ValueTask.FromResult(Failure());

        bool isMatch;

        try
        {
            isMatch = _regex.IsMatch(text);
        }
        catch (RegexMatchTimeoutException)
        {
            return ValueTask.FromResult(Failure());
        }

        return ValueTask.FromResult(isMatch == _shouldMatch ? ValidationRuleResult.Success() : Failure());
    }

    /// <summary>
    /// Builds the failure result, passing the description as a message parameter when one was supplied.
    /// </summary>
    ///
    /// <returns>The validation failure result.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private ValidationRuleResult Failure() => _description is null
        ? ValidationRuleResult.Failure(GetMessageKey())
        : ValidationRuleResult.Failure(GetMessageKey(), _description);

    /// <summary>
    /// Maps the configured mode onto the message key its failure reports, so one rule class serves every spelling of its family without
    /// each needing a class of its own.
    /// </summary>
    ///
    /// <returns>The validation message key.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private string GetMessageKey() => _shouldMatch ? "validation.regex" : "validation.not.regex";
}
