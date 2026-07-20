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
    /// Adds a rule that validates the property as an email address.
    /// </summary>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Email()
    {
        _propertyRule.AddRule(new FormatValidationRule<TRequest, TProperty>(FormatMode.Email));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property as a hex color.
    /// </summary>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> HexColor()
    {
        _propertyRule.AddRule(new FormatValidationRule<TRequest, TProperty>(FormatMode.HexColor));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property as a UUID.
    /// </summary>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Uuid()
    {
        _propertyRule.AddRule(new FormatValidationRule<TRequest, TProperty>(FormatMode.Uuid));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property as a ULID.
    /// </summary>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Ulid()
    {
        _propertyRule.AddRule(new FormatValidationRule<TRequest, TProperty>(FormatMode.Ulid));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property as an IPv4 or IPv6 address.
    /// </summary>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Ip()
    {
        _propertyRule.AddRule(new IpValidationRule<TRequest, TProperty>(IpMode.Any));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property as an IPv4 address.
    /// </summary>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Ipv4()
    {
        _propertyRule.AddRule(new IpValidationRule<TRequest, TProperty>(IpMode.Ipv4));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property as an IPv6 address.
    /// </summary>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Ipv6()
    {
        _propertyRule.AddRule(new IpValidationRule<TRequest, TProperty>(IpMode.Ipv6));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property as a MAC address.
    /// </summary>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> MacAddress()
    {
        _propertyRule.AddRule(new FormatValidationRule<TRequest, TProperty>(FormatMode.MacAddress));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property as JSON text.
    /// </summary>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Json()
    {
        _propertyRule.AddRule(new FormatValidationRule<TRequest, TProperty>(FormatMode.Json));

        return this;
    }

    /// <summary>
    /// Adds a rule that validates the property as an HTTP or HTTPS URL.
    /// </summary>
    ///
    /// <returns>The current rule builder.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Url()
    {
        _propertyRule.AddRule(new FormatValidationRule<TRequest, TProperty>(FormatMode.Url));

        return this;
    }
}
