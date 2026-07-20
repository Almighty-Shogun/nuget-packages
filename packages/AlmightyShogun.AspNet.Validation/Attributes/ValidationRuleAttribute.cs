using System.Reflection;
using System.Globalization;

namespace AlmightyShogun.AspNet.Validation;

/// <summary>
/// Base attribute used by validation attributes to create property validation rules.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public abstract class ValidationRuleAttribute : Attribute
{
    /// <summary>
    /// Stores the validation mode used by the configured rule type.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly object? _mode;

    /// <summary>
    /// Stores the primary numeric comparison value used by numeric, size, digit, and file rules.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly decimal _value;

    /// <summary>
    /// Stores a related field name, target field name, or literal date target.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly string? _field;

    /// <summary>
    /// Stores related field names for multi-field presence rules.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly string[]? _fields;

    /// <summary>
    /// Stores the optional upper comparison value used by range-like rules.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly decimal? _maxValue;

    /// <summary>
    /// Indicates whether a date comparison target refers to another request property.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly bool _targetIsProperty;

    /// <summary>
    /// Stores the kind of validation rule this attribute creates.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly AttributeRuleType _ruleType;

    /// <summary>
    /// Stores string comparison values, allowed file extensions, or MIME type values.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly IReadOnlyList<string>? _values;

    /// <summary>
    /// Stores comparison values for conditional field rules.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly IReadOnlyList<object?>? _conditionValues;

    /// <summary>
    /// Stores image dimension constraints for file validation rules.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly ImageDimensionConstraints? _dimensionConstraints;

    /// <summary>
    /// Creates a validation attribute without a preconfigured rule type.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private protected ValidationRuleAttribute() { }

    /// <summary>
    /// Creates a validation attribute for a presence rule.
    /// </summary>
    ///
    /// <param name="mode">The presence validation mode.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private protected ValidationRuleAttribute(PresenceMode mode)
    {
        _mode = mode;
        _ruleType = AttributeRuleType.Presence;
    }

    /// <summary>
    /// Creates a validation attribute for a type rule.
    /// </summary>
    ///
    /// <param name="mode">The type validation mode.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private protected ValidationRuleAttribute(TypeMode mode)
    {
        _mode = mode;
        _ruleType = AttributeRuleType.Type;
    }

    /// <summary>
    /// Creates a validation attribute for a format rule.
    /// </summary>
    ///
    /// <param name="mode">The format validation mode.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private protected ValidationRuleAttribute(FormatMode mode)
    {
        _mode = mode;
        _ruleType = AttributeRuleType.Format;
    }

    /// <summary>
    /// Creates a validation attribute for an IP address rule.
    /// </summary>
    ///
    /// <param name="mode">The IP validation mode.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private protected ValidationRuleAttribute(IpMode mode)
    {
        _mode = mode;
        _ruleType = AttributeRuleType.Ip;
    }

    /// <summary>
    /// Creates a validation attribute for a string character rule.
    /// </summary>
    ///
    /// <param name="mode">The string character validation mode.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private protected ValidationRuleAttribute(StringCharacterMode mode)
    {
        _mode = mode;
        _ruleType = AttributeRuleType.StringCharacter;
    }

    /// <summary>
    /// Creates a validation attribute for a string matching rule.
    /// </summary>
    ///
    /// <param name="mode">The string matching mode.</param>
    /// <param name="values">The comparison values.</param>
    /// <param name="doesNot">Whether to create the inverse matching rule.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private protected ValidationRuleAttribute(StringMatchMode mode, IReadOnlyList<string> values, bool doesNot = false)
    {
        _mode = mode;
        _values = values;
        _ruleType = doesNot ? AttributeRuleType.DoesNot : AttributeRuleType.StringMatch;
    }

    /// <summary>
    /// Creates a validation attribute for a comparable size rule.
    /// </summary>
    ///
    /// <param name="mode">The comparable size validation mode.</param>
    /// <param name="value">The primary comparison value.</param>
    /// <param name="maxValue">The optional maximum comparison value.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private protected ValidationRuleAttribute(ComparableSizeMode mode, double value, double? maxValue = null)
    {
        _mode = mode;
        _value = (decimal)value;
        _ruleType = AttributeRuleType.ComparableSize;
        _maxValue = maxValue is null ? null : (decimal)maxValue.Value;
    }

    /// <summary>
    /// Creates a validation attribute for a digit-count rule.
    /// </summary>
    ///
    /// <param name="mode">The digit-count validation mode.</param>
    /// <param name="value">The primary digit count.</param>
    /// <param name="maxValue">The optional maximum digit count.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private protected ValidationRuleAttribute(DigitMode mode, int value, int? maxValue = null)
    {
        _mode = mode;
        _value = value;
        _maxValue = maxValue;
        _ruleType = AttributeRuleType.Digit;
    }

    /// <summary>
    /// Creates a validation attribute for a file constraint rule.
    /// </summary>
    ///
    /// <param name="mode">The file constraint mode.</param>
    /// <param name="values">The optional comparison values.</param>
    /// <param name="dimensionConstraints">The optional image dimension constraints.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private protected ValidationRuleAttribute(FileConstraintMode mode, IReadOnlyList<string>? values = null, ImageDimensionConstraints? dimensionConstraints = null)
    {
        _mode = mode;
        _values = values;
        _ruleType = AttributeRuleType.File;
        _dimensionConstraints = dimensionConstraints;
    }

    /// <summary>
    /// Creates a validation attribute for a file dimension rule.
    /// </summary>
    ///
    /// <param name="mode">The file constraint mode.</param>
    /// <param name="width">The configured image width.</param>
    /// <param name="height">The configured image height.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private protected ValidationRuleAttribute(FileConstraintMode mode, int width, int height)
    {
        _mode = mode;
        _ruleType = AttributeRuleType.File;
        _dimensionConstraints = new ImageDimensionConstraints(width, height);
    }

    /// <summary>
    /// Creates a validation attribute for a field comparison rule.
    /// </summary>
    ///
    /// <param name="mode">The field comparison mode.</param>
    /// <param name="field">The field to compare against.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private protected ValidationRuleAttribute(FieldComparisonMode mode, string field)
    {
        _mode = mode;
        _field = field;
        _ruleType = AttributeRuleType.FieldComparison;
    }

    /// <summary>
    /// Creates a validation attribute for a conditional rule.
    /// </summary>
    ///
    /// <param name="targetMode">The target validation mode.</param>
    /// <param name="conditionMode">The condition mode.</param>
    /// <param name="field">The conditional field.</param>
    /// <param name="values">The conditional values.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private protected ValidationRuleAttribute(ConditionalTargetMode targetMode, ConditionMode conditionMode, string field, IReadOnlyList<object?> values)
    {
        _field = field;
        _conditionValues = values;
        _mode = (targetMode, conditionMode);
        _ruleType = AttributeRuleType.Conditional;
    }

    /// <summary>
    /// Creates a validation attribute for a conditional state rule.
    /// </summary>
    ///
    /// <param name="targetMode">The target validation mode.</param>
    /// <param name="stateMode">The conditional state mode.</param>
    /// <param name="field">The conditional field.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private protected ValidationRuleAttribute(ConditionalStateTargetMode targetMode, ConditionalStateMode stateMode, string field)
    {
        _field = field;
        _mode = (targetMode, stateMode);
        _ruleType = AttributeRuleType.ConditionalState;
    }

    /// <summary>
    /// Creates a validation attribute for a multi-field presence rule.
    /// </summary>
    ///
    /// <param name="targetMode">The target validation mode.</param>
    /// <param name="triggerMode">The multi-field trigger mode.</param>
    /// <param name="fields">The related fields.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private protected ValidationRuleAttribute(MultiFieldPresenceTargetMode targetMode, MultiFieldPresenceTriggerMode triggerMode, string[] fields)
    {
        _fields = fields;
        _mode = (targetMode, triggerMode);
        _ruleType = AttributeRuleType.MultiFieldPresence;
    }

    /// <summary>
    /// Creates a validation attribute for a date comparison rule.
    /// </summary>
    ///
    /// <param name="mode">The date comparison mode.</param>
    /// <param name="target">The literal date or target property name.</param>
    /// <param name="targetIsProperty">Whether the target refers to another property.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private protected ValidationRuleAttribute(DateMode mode, string target, bool targetIsProperty)
    {
        _mode = mode;
        _field = target;
        _targetIsProperty = targetIsProperty;
        _ruleType = AttributeRuleType.DateComparison;
    }

    /// <summary>
    /// Creates the property validation rule represented by this attribute.
    /// </summary>
    ///
    /// <param name="property">The property decorated with the validation attribute.</param>
    ///
    /// <returns>The configured property validation rule.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal virtual IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property) where TRequest : class => _ruleType switch
    {
        AttributeRuleType.Presence => CreatePresenceRule<TRequest, TProperty>((PresenceMode)_mode!),
        AttributeRuleType.Type => CreateTypeRule<TRequest, TProperty>((TypeMode)_mode!),
        AttributeRuleType.Format => CreateFormatRule<TRequest, TProperty>((FormatMode)_mode!),
        AttributeRuleType.Ip => CreateIpRule<TRequest, TProperty>((IpMode)_mode!),
        AttributeRuleType.StringCharacter => CreateStringCharacterRule<TRequest, TProperty>((StringCharacterMode)_mode!),
        AttributeRuleType.StringMatch => CreateStringMatchRule<TRequest, TProperty>((StringMatchMode)_mode!, _values!),
        AttributeRuleType.DoesNot => CreateDoesNotRule<TRequest, TProperty>((StringMatchMode)_mode!, _values!),
        AttributeRuleType.ComparableSize => CreateComparableSizeRule<TRequest, TProperty>((ComparableSizeMode)_mode!, _value, _maxValue),
        AttributeRuleType.Digit => CreateDigitRule<TRequest, TProperty>((DigitMode)_mode!, (int)_value, _maxValue is null ? null : (int)_maxValue.Value),
        AttributeRuleType.File => CreateFileRule<TRequest, TProperty>((FileConstraintMode)_mode!, _values, _dimensionConstraints),
        AttributeRuleType.FieldComparison => new FieldComparisonValidationRule<TRequest, TProperty, object?>((FieldComparisonMode)_mode!, _field!),
        AttributeRuleType.Conditional => CreateConditionalRule<TRequest, TProperty>(),
        AttributeRuleType.ConditionalState => CreateConditionalStateRule<TRequest, TProperty>(),
        AttributeRuleType.MultiFieldPresence => CreateMultiFieldPresenceRule<TRequest, TProperty>(),
        AttributeRuleType.DateComparison => CreateDateComparisonRule<TRequest, TProperty>(),
        _ => throw new InvalidOperationException($"The validation attribute '{GetType().Name}' does not define a validation rule.")
    };

    /// <summary>
    /// Creates a presence validation rule.
    /// </summary>
    ///
    /// <param name="mode">The presence validation mode.</param>
    ///
    /// <returns>The presence validation rule.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static PresenceValidationRule<TRequest, TProperty> CreatePresenceRule<TRequest, TProperty>(PresenceMode mode)
        where TRequest : class => new(mode);

    /// <summary>
    /// Creates a type validation rule.
    /// </summary>
    ///
    /// <param name="mode">The type validation mode.</param>
    ///
    /// <returns>The type validation rule.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static TypeValidationRule<TRequest, TProperty> CreateTypeRule<TRequest, TProperty>(TypeMode mode)
        where TRequest : class => new(mode);

    /// <summary>
    /// Creates a format validation rule.
    /// </summary>
    ///
    /// <param name="mode">The format validation mode.</param>
    ///
    /// <returns>The format validation rule.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static FormatValidationRule<TRequest, TProperty> CreateFormatRule<TRequest, TProperty>(FormatMode mode)
        where TRequest : class => new(mode);

    /// <summary>
    /// Creates an IP validation rule.
    /// </summary>
    ///
    /// <param name="mode">The IP validation mode.</param>
    ///
    /// <returns>The IP validation rule.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static IpValidationRule<TRequest, TProperty> CreateIpRule<TRequest, TProperty>(IpMode mode)
        where TRequest : class => new(mode);

    /// <summary>
    /// Creates a string character validation rule.
    /// </summary>
    ///
    /// <param name="mode">The string character validation mode.</param>
    ///
    /// <returns>The string character validation rule.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static StringCharacterValidationRule<TRequest, TProperty> CreateStringCharacterRule<TRequest, TProperty>(
        StringCharacterMode mode)
        where TRequest : class => new(mode);

    /// <summary>
    /// Creates a string matching validation rule.
    /// </summary>
    ///
    /// <param name="mode">The string matching mode.</param>
    /// <param name="values">The comparison values.</param>
    ///
    /// <returns>The string matching validation rule.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static StringMatchValidationRule<TRequest, TProperty> CreateStringMatchRule<TRequest, TProperty>(
        StringMatchMode mode, IReadOnlyList<string> values)
        where TRequest : class => new(mode, values);

    /// <summary>
    /// Creates an inverse string matching validation rule.
    /// </summary>
    ///
    /// <param name="mode">The string matching mode.</param>
    /// <param name="values">The forbidden comparison values.</param>
    ///
    /// <returns>The inverse string matching validation rule.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static DoesNotValidationRule<TRequest, TProperty> CreateDoesNotRule<TRequest, TProperty>(
        StringMatchMode mode, IReadOnlyList<string> values)
        where TRequest : class => new(mode, values);

    /// <summary>
    /// Creates a comparable size validation rule.
    /// </summary>
    ///
    /// <param name="mode">The comparable size validation mode.</param>
    /// <param name="value">The primary comparison value.</param>
    /// <param name="maxValue">The optional maximum comparison value.</param>
    ///
    /// <returns>The comparable size validation rule.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static ComparableSizeValidationRule<TRequest, TProperty> CreateComparableSizeRule<TRequest, TProperty>(
        ComparableSizeMode mode, decimal value, decimal? maxValue = null)
        where TRequest : class => new(mode, value, maxValue);

    /// <summary>
    /// Creates a digit-count validation rule.
    /// </summary>
    ///
    /// <param name="mode">The digit-count validation mode.</param>
    /// <param name="value">The primary digit count.</param>
    /// <param name="maxValue">The optional maximum digit count.</param>
    ///
    /// <returns>The digit-count validation rule.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static DigitCountValidationRule<TRequest, TProperty> CreateDigitRule<TRequest, TProperty>(
        DigitMode mode, int value, int? maxValue = null)
        where TRequest : class => new(mode, value, maxValue);

    /// <summary>
    /// Creates a file constraint validation rule.
    /// </summary>
    ///
    /// <param name="mode">The file constraint mode.</param>
    /// <param name="values">The optional comparison values.</param>
    /// <param name="dimensionConstraints">The optional image dimension constraints.</param>
    ///
    /// <returns>The file constraint validation rule.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static FileConstraintValidationRule<TRequest, TProperty> CreateFileRule<TRequest, TProperty>(
        FileConstraintMode mode,
        IReadOnlyList<string>? values = null,
        ImageDimensionConstraints? dimensionConstraints = null)
        where TRequest : class => new(mode, values, dimensionConstraints);

    /// <summary>
    /// Creates a conditional validation rule from stored attribute metadata.
    /// </summary>
    ///
    /// <returns>The conditional validation rule.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private IPropertyValidationRule<TRequest, TProperty> CreateConditionalRule<TRequest, TProperty>() where TRequest : class
    {
        (ConditionalTargetMode targetMode, ConditionMode conditionMode) = ((ConditionalTargetMode, ConditionMode))_mode!;

        return new NamedConditionalValidationRule<TRequest, TProperty>(targetMode, conditionMode, _field!, _conditionValues!);
    }

    /// <summary>
    /// Creates a conditional state validation rule from stored attribute metadata.
    /// </summary>
    ///
    /// <returns>The conditional state validation rule.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private IPropertyValidationRule<TRequest, TProperty> CreateConditionalStateRule<TRequest, TProperty>() where TRequest : class
    {
        (ConditionalStateTargetMode targetMode, ConditionalStateMode stateMode) = ((ConditionalStateTargetMode, ConditionalStateMode))_mode!;

        return new NamedConditionalStateValidationRule<TRequest, TProperty>(targetMode, stateMode, _field!);
    }

    /// <summary>
    /// Creates a multi-field presence validation rule from stored attribute metadata.
    /// </summary>
    ///
    /// <returns>The multi-field presence validation rule.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private IPropertyValidationRule<TRequest, TProperty> CreateMultiFieldPresenceRule<TRequest, TProperty>() where TRequest : class
    {
        (MultiFieldPresenceTargetMode targetMode, MultiFieldPresenceTriggerMode triggerMode) = ((MultiFieldPresenceTargetMode, MultiFieldPresenceTriggerMode))_mode!;

        return new NamedMultiFieldPresenceValidationRule<TRequest, TProperty>(targetMode, triggerMode, _fields!);
    }

    /// <summary>
    /// Creates a date comparison validation rule from stored attribute metadata.
    /// </summary>
    ///
    /// <returns>The date comparison validation rule.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private IPropertyValidationRule<TRequest, TProperty> CreateDateComparisonRule<TRequest, TProperty>() where TRequest : class
    {
        var mode = (DateMode)_mode!;

        return _targetIsProperty
            ? new DateValidationRule<TRequest, TProperty>(mode, _field!)
            : new DateValidationRule<TRequest, TProperty>(mode, DateTimeOffset.Parse(_field!, CultureInfo.InvariantCulture));
    }

}
