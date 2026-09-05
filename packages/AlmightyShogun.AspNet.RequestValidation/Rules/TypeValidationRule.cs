namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Checks that a bound value is the shape the rule names. An absent or empty value satisfies <see cref="TypeMode.Array"/>,
/// <see cref="TypeMode.List"/> and <see cref="TypeMode.File"/>, which short-circuit on it, while <see cref="TypeMode.String"/> and
/// <see cref="TypeMode.Boolean"/> apply their own narrower notion instead, so an empty collection fails the string check.
/// </summary>
///
/// <typeparam name="TRequest">The request type the rule is declared on, though only the bound value decides the outcome.</typeparam>
/// <typeparam name="TProperty">The bound property's type, which does not decide the check: the value's runtime shape does.</typeparam>
/// <param name="mode">Which shape the value must have, which also decides the message a failure reports.</param>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class TypeValidationRule<TRequest, TProperty>(
    TypeMode mode
) : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    /// <inheritdoc />
    public ValueTask<ValidationRuleResult> ValidateAsync(
        TRequest request,
        TProperty? value,
        string field,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default
    )
    {
        bool isValid = mode switch
        {
            TypeMode.String => ValidationValue.IsString(value),
            TypeMode.Boolean => ValidationValue.IsBoolean(value),
            TypeMode.Array => ValidationValue.IsEmpty(value) || ValidationCollection.IsArrayLike(value),
            TypeMode.List => ValidationValue.IsEmpty(value) || ValidationCollection.IsListLike(value),
            TypeMode.File => ValidationValue.IsEmpty(value) || ValidationFile.TryGetFiles(value, out _),
            _ => throw new InvalidOperationException($"Unsupported TypeMode value '{mode}'.")
        };

        return ValueTask.FromResult(isValid ? ValidationRuleResult.Success() : ValidationRuleResult.Failure(GetMessageKey()));
    }

    /// <summary>
    /// Maps the configured mode onto the message key its failure reports, so one rule class serves every spelling of its family without
    /// each needing a class of its own.
    /// </summary>
    ///
    /// <returns>The validation message key.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private string GetMessageKey() => mode switch
    {
        TypeMode.String => "validation.string",
        TypeMode.Boolean => "validation.boolean",
        TypeMode.Array => "validation.array",
        TypeMode.List => "validation.list",
        TypeMode.File => "validation.file",
        _ => throw new InvalidOperationException($"Unsupported TypeMode value '{mode}'.")
    };
}
