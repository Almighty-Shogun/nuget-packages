namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Builds the rule set for one request property. Split across several partial files by rule family, so the fluent surface stays one type
/// while each family's methods sit together.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public sealed partial class RuleBuilder<TRequest, TProperty> where TRequest : class
{
    /// <summary>
    /// Requires the value to be an uploaded file. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> File()
    {
        _propertyRule.AddRule(new TypeValidationRule<TRequest, TProperty>(TypeMode.File));

        return this;
    }

    /// <summary>
    /// Requires the uploaded file to be present and non-empty. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Uploaded()
    {
        _propertyRule.AddRule(new FileConstraintValidationRule<TRequest, TProperty>(FileConstraintMode.Uploaded));

        return this;
    }

    /// <summary>
    /// Allows only files with one of the provided file extensions. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="extensions">The allowed file extensions.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Extensions(params string[] extensions)
    {
        _propertyRule.AddRule(new FileConstraintValidationRule<TRequest, TProperty>(FileConstraintMode.Extensions, extensions));

        return this;
    }

    /// <summary>
    /// Allows only files matching the provided MIME extension aliases. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="mimes">The allowed MIME aliases or extensions.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Mimes(params string[] mimes)
    {
        _propertyRule.AddRule(new FileConstraintValidationRule<TRequest, TProperty>(FileConstraintMode.Mimes, mimes));

        return this;
    }

    /// <summary>
    /// Allows only files with one of the provided MIME types. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="mimeTypes">The allowed MIME types.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> MimeTypes(params string[] mimeTypes)
    {
        _propertyRule.AddRule(new FileConstraintValidationRule<TRequest, TProperty>(FileConstraintMode.MimeTypes, mimeTypes));

        return this;
    }

    /// <summary>
    /// Requires the uploaded file to be an image. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Image()
    {
        _propertyRule.AddRule(new FileConstraintValidationRule<TRequest, TProperty>(FileConstraintMode.Image));

        return this;
    }

    /// <summary>
    /// Requires the uploaded image to match the exact width and height. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="width">The required image width.</param>
    /// <param name="height">The required image height.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Dimensions(int width, int height)
    {
        _propertyRule.AddRule(
            new FileConstraintValidationRule<TRequest, TProperty>(
                FileConstraintMode.Dimensions,
                null,
                new ImageDimensionConstraints(width, height)
            )
        );

        return this;
    }

    /// <summary>
    /// Requires the uploaded image to be at least the provided width and height. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="width">The minimum image width.</param>
    /// <param name="height">The minimum image height.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> MinDimensions(int width, int height)
    {
        _propertyRule.AddRule(
            new FileConstraintValidationRule<TRequest, TProperty>(
                FileConstraintMode.MinDimensions,
                null,
                new ImageDimensionConstraints(width, height)
            )
        );

        return this;
    }

    /// <summary>
    /// Requires the uploaded image to be no larger than the provided width and height. An absent or empty value passes, so pair it with
    /// <see cref="RuleBuilder{TRequest,TProperty}.Required"/> when the field is mandatory.
    /// </summary>
    ///
    /// <param name="width">The maximum image width.</param>
    /// <param name="height">The maximum image height.</param>
    ///
    /// <returns>
    /// The same builder, so rules chain. Order of declaration is preserved, which is what lets a presence rule run before the value rules
    /// that follow it.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> MaxDimensions(int width, int height)
    {
        _propertyRule.AddRule(
            new FileConstraintValidationRule<TRequest, TProperty>(
                FileConstraintMode.MaxDimensions,
                null,
                new ImageDimensionConstraints(width, height)
            )
        );

        return this;
    }
}
