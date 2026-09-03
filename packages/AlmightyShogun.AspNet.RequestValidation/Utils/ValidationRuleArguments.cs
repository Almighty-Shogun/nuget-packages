namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Checks the arguments a rule was declared with, so a rule that could never do anything useful is refused where it is written rather than
/// silently passing or failing every request that reaches it.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal static class ValidationRuleArguments
{
    /// <summary>
    /// Accepts a set of configured values only when it holds at least one.
    /// </summary>
    ///
    /// <typeparam name="TValue">The kind of value the rule compares against.</typeparam>
    /// <param name="values">The values the rule was declared with.</param>
    /// <param name="parameterName">The parameter to name in the failure, so the caller is told which argument was empty.</param>
    ///
    /// <returns>The values, once they are known not to be empty.</returns>
    ///
    /// <exception cref="ArgumentOutOfRangeException">
    /// The set is empty. An empty set makes a rule vacuous rather than lenient, and which way it goes depends on whether the rule asks
    /// that any value match or that none does: a membership rule would reject everything and a forbidding rule would accept everything.
    /// Neither is what declaring the rule meant, so it is refused as the rule is built.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static IReadOnlyList<TValue> RequireAny<TValue>(IReadOnlyList<TValue> values, string parameterName)
        => values.Count > 0
            ? values
            : throw new ArgumentOutOfRangeException(parameterName, "A validation rule comparing against a set needs at least one value.");
}
