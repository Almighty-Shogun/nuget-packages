namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Builds validation rules for a request property.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed partial class RuleBuilder<TRequest, TProperty> where TRequest : class
{
    /// <summary>
    /// Adds a rule that validates the property as an uploaded file value.
    /// </summary>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> File()
    {
        _propertyRule.AddRule(new TypeValidationRule<TRequest, TProperty>(TypeMode.File));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the uploaded file completed successfully.
    /// </summary>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Uploaded()
    {
        _propertyRule.AddRule(new FileConstraintValidationRule<TRequest, TProperty>(FileConstraintMode.Uploaded));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates uploaded files against allowed file extensions.
    /// </summary>
    ///
    /// <param name="extensions">The allowed file extensions.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Extensions(params string[] extensions)
    {
        _propertyRule.AddRule(new FileConstraintValidationRule<TRequest, TProperty>(FileConstraintMode.Extensions, extensions));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates uploaded files against MIME types inferred from extensions.
    /// </summary>
    ///
    /// <param name="mimes">The allowed MIME aliases or extensions.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Mimes(params string[] mimes)
    {
        _propertyRule.AddRule(new FileConstraintValidationRule<TRequest, TProperty>(FileConstraintMode.Mimes, mimes));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates uploaded files against explicit MIME types.
    /// </summary>
    ///
    /// <param name="mimeTypes">The allowed MIME types.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> MimeTypes(params string[] mimeTypes)
    {
        _propertyRule.AddRule(new FileConstraintValidationRule<TRequest, TProperty>(FileConstraintMode.MimeTypes, mimeTypes));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates uploaded files as images.
    /// </summary>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Image()
    {
        _propertyRule.AddRule(new FileConstraintValidationRule<TRequest, TProperty>(FileConstraintMode.Image));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates image files against exact dimensions.
    /// </summary>
    ///
    /// <param name="width">The required image width.</param>
    /// <param name="height">The required image height.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Dimensions(int width, int height)
    {
        _propertyRule.AddRule(new FileConstraintValidationRule<TRequest, TProperty>(
            FileConstraintMode.Dimensions,
            null,
            new ImageDimensionConstraints(width, height)));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates image files against minimum dimensions.
    /// </summary>
    ///
    /// <param name="width">The minimum image width.</param>
    /// <param name="height">The minimum image height.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> MinDimensions(int width, int height)
    {
        _propertyRule.AddRule(new FileConstraintValidationRule<TRequest, TProperty>(
            FileConstraintMode.MinDimensions,
            null,
            new ImageDimensionConstraints(width, height)));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates image files against maximum dimensions.
    /// </summary>
    ///
    /// <param name="width">The maximum image width.</param>
    /// <param name="height">The maximum image height.</param>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> MaxDimensions(int width, int height)
    {
        _propertyRule.AddRule(new FileConstraintValidationRule<TRequest, TProperty>(
            FileConstraintMode.MaxDimensions,
            null,
            new ImageDimensionConstraints(width, height)));

        return this;
    }
}
