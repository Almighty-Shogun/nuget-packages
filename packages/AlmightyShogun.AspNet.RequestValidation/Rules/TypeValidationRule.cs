namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Checks that a bound value is the shape the rule names. An absent or empty value passes, so it never implies the field is required.
/// </summary>
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
