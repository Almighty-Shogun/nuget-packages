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
    /// Requires the value to be a valid email address.
    /// </summary>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Email()
    {
        _propertyRule.AddRule(new FormatValidationRule<TRequest, TProperty>(FormatMode.Email));

        return this;
    }

    /// <summary>
    /// Requires the value to be a hexadecimal color value.
    /// </summary>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> HexColor()
    {
        _propertyRule.AddRule(new FormatValidationRule<TRequest, TProperty>(FormatMode.HexColor));

        return this;
    }

    /// <summary>
    /// Requires the value to be a valid UUID value.
    /// </summary>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Uuid()
    {
        _propertyRule.AddRule(new FormatValidationRule<TRequest, TProperty>(FormatMode.Uuid));

        return this;
    }

    /// <summary>
    /// Requires the value to be a valid ULID value.
    /// </summary>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Ulid()
    {
        _propertyRule.AddRule(new FormatValidationRule<TRequest, TProperty>(FormatMode.Ulid));

        return this;
    }

    /// <summary>
    /// Requires the value to be a valid IPv4 or IPv6 address.
    /// </summary>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Ip()
    {
        _propertyRule.AddRule(new IpValidationRule<TRequest, TProperty>(IpMode.Any));

        return this;
    }

    /// <summary>
    /// Requires the value to be a valid IPv4 address.
    /// </summary>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Ipv4()
    {
        _propertyRule.AddRule(new IpValidationRule<TRequest, TProperty>(IpMode.Ipv4));

        return this;
    }

    /// <summary>
    /// Requires the value to be a valid IPv6 address.
    /// </summary>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Ipv6()
    {
        _propertyRule.AddRule(new IpValidationRule<TRequest, TProperty>(IpMode.Ipv6));

        return this;
    }

    /// <summary>
    /// Requires the value to be a valid MAC address.
    /// </summary>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> MacAddress()
    {
        _propertyRule.AddRule(new FormatValidationRule<TRequest, TProperty>(FormatMode.MacAddress));

        return this;
    }

    /// <summary>
    /// Requires the value to be valid JSON text.
    /// </summary>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Json()
    {
        _propertyRule.AddRule(new FormatValidationRule<TRequest, TProperty>(FormatMode.Json));

        return this;
    }

    /// <summary>
    /// Requires the value to be an HTTP or HTTPS URL.
    /// </summary>
    ///
    /// <returns>The same builder, so rules chain.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public RuleBuilder<TRequest, TProperty> Url()
    {
        _propertyRule.AddRule(new FormatValidationRule<TRequest, TProperty>(FormatMode.Url));

        return this;
    }
}
