namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Validates type constraints for a request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
internal sealed class TypeValidationRule<TRequest, TProperty>(TypeMode mode) : IPropertyValidationRule<TRequest, TProperty> where TRequest : class
{
    /// <inheritdoc />
    public ValueTask<ValidationRuleResult> ValidateAsync(TRequest request, TProperty? value, string field, IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        bool isValid = mode switch
        {
            TypeMode.String => ValidationValue.IsString(value),
            TypeMode.Boolean => ValidationValue.IsBoolean(value),
            TypeMode.Array => ValidationValue.IsEmpty(value) || ValidationCollection.IsArrayLike(value),
            TypeMode.List => ValidationValue.IsEmpty(value) || ValidationCollection.IsListLike(value),
            TypeMode.File => ValidationValue.IsEmpty(value) || ValidationFile.TryGetFiles(value, out _),
            _ => false
        };

        return ValueTask.FromResult(isValid
            ? ValidationRuleResult.Success()
            : ValidationRuleResult.Failure(GetMessageKey()));
    }

    /// <summary>
    /// Gets the validation message key for the configured type mode.
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
        _ => "validation.file"
    };
}
