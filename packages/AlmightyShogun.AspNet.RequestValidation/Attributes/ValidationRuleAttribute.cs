using System.Reflection;
using System.Globalization;

namespace AlmightyShogun.AspNet.RequestValidation;

/// <summary>
/// Base attribute every validation attribute derives from. It carries the arguments an attribute may legally hold and turns them into a
/// rule the first time its request type is validated, so each derived attribute is a declaration rather than an implementation.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>Unreleased</since>
public abstract class ValidationRuleAttribute : Attribute
{
    /// <summary>
    /// The mode selecting the behaviour within a rule family. Typed as <see cref="object"/> because each family has its own enum, and three
    /// families store a tuple of two, so the boxing happens once when the runtime builds the attribute rather than per request.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly object? _mode;

    /// <summary>
    /// The primary comparison value for numeric, size, digit, and file rules. Held as <see cref="decimal"/> although the constructors take
    /// <see cref="double"/> and <see cref="int"/> , because an attribute argument cannot be a decimal and the comparison wants the
    /// precision.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly decimal _value;

    /// <summary>
    /// The other field a rule reads, or the literal date a date rule compares against. Which of the two it holds is decided by
    /// <see cref="_ruleType"/> , and for a date rule by <see cref="_targetIsProperty"/> .
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly string? _field;

    /// <summary>
    /// The other fields a multi-field presence rule watches. Only that family sets it, so it is <c>null</c> for every other rule.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly string[]? _fields;

    /// <summary>
    /// The upper bound for a range rule such as between or digits-between. <c>null</c> for the one-sided rules that share the same family.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly decimal? _maxValue;

    /// <summary>
    /// Whether a date rule's target names another property rather than being a literal date. Without it the two cases are
    /// indistinguishable, since both arrive as a string.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly bool _targetIsProperty;

    /// <summary>
    /// The family this attribute belongs to, which is what decides how <see cref="_mode"/> is cast back. The two are always assigned
    /// together in a constructor, so a mismatch is impossible by construction rather than by check.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly AttributeRuleType _ruleType;

    /// <summary>
    /// The literal strings a rule compares against: the values to match, the permitted extensions, or the permitted MIME types.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly IReadOnlyList<string>? _values;

    /// <summary>
    /// The values the controlling field is compared against by a conditional rule. Typed loosely because a condition may name a number, a
    /// string, or a boolean, all of which are legal attribute arguments.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly IReadOnlyList<object?>? _conditionValues;

    /// <summary>
    /// The width and height an uploaded image is checked against. Set only by the file family, and only by the dimension rules within it.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private readonly ImageDimensionConstraints? _dimensionConstraints;

    /// <summary>
    /// Creates an attribute that builds its own rule. Used by the few attributes that override <see cref="CreateRule{TRequest,TProperty}"/>
    /// because their rule needs the property itself, which this base never sees.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private protected ValidationRuleAttribute() { }

    /// <summary>
    /// Configures a presence rule: whether the field must exist, hold a value, be absent, or be forbidden. These run ahead of value rules
    /// so a missing field reports that rather than a later format failure.
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
    /// Configures a type rule, which checks that the bound value is the shape the rule names before any rule inspects its contents.
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
    /// Configures a format rule, matching the value against a known textual shape such as an email address, a URL, or a UUID.
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
    /// Configures an IP rule, accepting either address family or restricting the value to one of them.
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
    /// Configures a character-class rule, restricting the text to letters, digits, or a case the value must already be in.
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
    /// Configures a substring rule. The same mode set serves both directions, with the flag inverting the result rather than a second mode
    /// existing for it.
    /// </summary>
    ///
    /// <param name="mode">Where in the text the value must appear, shared with the negated spelling of the same family.</param>
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
    /// Configures a size rule. What is measured depends on the bound type: a number's value, a string's length, a collection's count, or a
    /// file's size.
    /// </summary>
    ///
    /// <param name="mode">The comparable size validation mode.</param>
    /// <param name="value">The bound compared against, or the lower of the two when the rule takes a range.</param>
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
    /// Configures a digit-count rule, counting the digits in the value rather than reading it as a number, so leading zeroes count.
    /// </summary>
    ///
    /// <param name="mode">The digit-count validation mode.</param>
    /// <param name="value">The digit count required, or the lower of the two when the rule takes a range.</param>
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
    /// Configures an upload rule, limiting the permitted extensions or MIME types, or requiring the file to be an image.
    /// </summary>
    ///
    /// <param name="mode">Which property of the upload is constrained, which also picks the message the failure reports.</param>
    /// <param name="values">The values compared against, absent for a rule whose constraint needs none.</param>
    /// <param name="dimensionConstraints">The width and height to enforce, absent for a file rule that constrains something else.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private protected ValidationRuleAttribute(
        FileConstraintMode mode,
        IReadOnlyList<string>? values = null,
        ImageDimensionConstraints? dimensionConstraints = null
    )
    {
        _mode = mode;
        _values = values;
        _ruleType = AttributeRuleType.File;
        _dimensionConstraints = dimensionConstraints;
    }

    /// <summary>
    /// Configures an exact image-dimension rule. The pair is stored as a constraint here so the general file constructor stays free of
    /// dimension arguments.
    /// </summary>
    ///
    /// <param name="mode">Which property of the upload is constrained, which also picks the message the failure reports.</param>
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
    /// Configures a rule comparing this field against another on the same request, such as a confirmation or a must-differ pairing.
    /// </summary>
    ///
    /// <param name="mode">How this field must relate to the other one, which also decides the message the failure reports.</param>
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
    /// Configures a rule that only applies when a controlling field holds one of the listed values. Both halves of the decision are stored,
    /// since the target and the condition vary independently.
    /// </summary>
    ///
    /// <param name="targetMode">
    /// What the rule does to its own field once the condition holds: demand a value, demand presence, or forbid it.
    /// </param>
    /// <param name="conditionMode">
    /// Whether the rule fires on a match or on the absence of one, which separates the if and unless spellings.
    /// </param>
    /// <param name="field">The conditional field.</param>
    /// <param name="values">The conditional values.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private protected ValidationRuleAttribute(
        ConditionalTargetMode targetMode,
        ConditionMode conditionMode,
        string field,
        IReadOnlyList<object?> values
    )
    {
        _field = field;
        _conditionValues = values;
        _mode = (targetMode, conditionMode);
        _ruleType = AttributeRuleType.Conditional;
    }

    /// <summary>
    /// Configures a rule that applies when a controlling field is accepted or declined, which is the truthiness check rather than an
    /// equality one.
    /// </summary>
    ///
    /// <param name="targetMode">
    /// What the rule does to its own field once the condition holds: demand a value, demand presence, or forbid it.
    /// </param>
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
    /// Configures a rule keyed on the presence of several other fields, applying when any or all of them are present or missing.
    /// </summary>
    ///
    /// <param name="targetMode">
    /// What the rule does to its own field once the condition holds: demand a value, demand presence, or forbid it.
    /// </param>
    /// <param name="triggerMode">The multi-field trigger mode.</param>
    /// <param name="fields">The related fields.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private protected ValidationRuleAttribute(
        MultiFieldPresenceTargetMode targetMode,
        MultiFieldPresenceTriggerMode triggerMode,
        string[] fields
    )
    {
        _fields = fields;
        _mode = (targetMode, triggerMode);
        _ruleType = AttributeRuleType.MultiFieldPresence;
    }

    /// <summary>
    /// Configures a date rule comparing the value against a literal date or another property, which is why the target's kind is stored
    /// alongside it.
    /// </summary>
    ///
    /// <param name="mode">Which ordering the value must satisfy against the target.</param>
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
    /// Builds the rule this attribute declares, once per request type on its first validation rather than per request. An attribute whose
    /// family was never set reaches the default branch and throws, which catches a derived attribute that forgot to call a base
    /// constructor.
    /// </summary>
    ///
    /// <param name="property">The property decorated with the validation attribute.</param>
    ///
    /// <returns>The configured property validation rule.</returns>
    ///
    /// <exception cref="InvalidOperationException">
    /// The attribute never set a rule family, which happens when a derived attribute does not call one of the base constructors.
    /// </exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal virtual IPropertyValidationRule<TRequest, TProperty> CreateRule<TRequest, TProperty>(PropertyInfo property)
        where TRequest : class => _ruleType switch
    {
        AttributeRuleType.Presence => CreatePresenceRule<TRequest, TProperty>((PresenceMode)_mode!),
        AttributeRuleType.Type => CreateTypeRule<TRequest, TProperty>((TypeMode)_mode!),
        AttributeRuleType.Format => CreateFormatRule<TRequest, TProperty>((FormatMode)_mode!),
        AttributeRuleType.Ip => CreateIpRule<TRequest, TProperty>((IpMode)_mode!),
        AttributeRuleType.StringCharacter => CreateStringCharacterRule<TRequest, TProperty>((StringCharacterMode)_mode!),
        AttributeRuleType.StringMatch => CreateStringMatchRule<TRequest, TProperty>((StringMatchMode)_mode!, _values!),
        AttributeRuleType.DoesNot => CreateDoesNotRule<TRequest, TProperty>((StringMatchMode)_mode!, _values!),
        AttributeRuleType.ComparableSize => CreateComparableSizeRule<TRequest, TProperty>((ComparableSizeMode)_mode!, _value, _maxValue),
        AttributeRuleType.Digit => CreateDigitRule<TRequest, TProperty>(
            (DigitMode)_mode!,
            (int)_value,
            _maxValue is null ? null : (int)_maxValue.Value
        ),
        AttributeRuleType.File => CreateFileRule<TRequest, TProperty>((FileConstraintMode)_mode!, _values, _dimensionConstraints),
        AttributeRuleType.FieldComparison => new FieldComparisonValidationRule<TRequest, TProperty, object?>(
            (FieldComparisonMode)_mode!, _field!
        ),
        AttributeRuleType.Conditional => CreateConditionalRule<TRequest, TProperty>(),
        AttributeRuleType.ConditionalState => CreateConditionalStateRule<TRequest, TProperty>(),
        AttributeRuleType.MultiFieldPresence => CreateMultiFieldPresenceRule<TRequest, TProperty>(),
        AttributeRuleType.DateComparison => CreateDateComparisonRule<TRequest, TProperty>(),
        _ => throw new InvalidOperationException($"The validation attribute '{GetType().Name}' does not define a validation rule.")
    };

    /// <summary>
    /// Builds the presence rule for the stored mode.
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
    /// Builds the type rule for the stored mode.
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
    /// Builds the format rule for the stored mode.
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
    /// Builds the IP rule for the stored mode.
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
    /// Builds the character-class rule for the stored mode.
    /// </summary>
    ///
    /// <param name="mode">The string character validation mode.</param>
    ///
    /// <returns>The string character validation rule.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static StringCharacterValidationRule<TRequest, TProperty> CreateStringCharacterRule<TRequest, TProperty>(
        StringCharacterMode mode
    ) where TRequest : class => new(mode);

    /// <summary>
    /// Builds the substring rule for the stored mode and values.
    /// </summary>
    ///
    /// <param name="mode">Where in the text the value must appear, shared with the negated spelling of the same family.</param>
    /// <param name="values">The comparison values.</param>
    ///
    /// <returns>The string matching validation rule.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static StringMatchValidationRule<TRequest, TProperty> CreateStringMatchRule<TRequest, TProperty>(
        StringMatchMode mode,
        IReadOnlyList<string> values
    ) where TRequest : class => new(mode, values);

    /// <summary>
    /// Builds the negated substring rule, which shares the mode set with the positive one and only differs in the result it reports.
    /// </summary>
    ///
    /// <param name="mode">Where in the text the value must appear, shared with the negated spelling of the same family.</param>
    /// <param name="values">The forbidden comparison values.</param>
    ///
    /// <returns>The inverse string matching validation rule.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static DoesNotValidationRule<TRequest, TProperty> CreateDoesNotRule<TRequest, TProperty>(
        StringMatchMode mode,
        IReadOnlyList<string> values
    ) where TRequest : class => new(mode, values);

    /// <summary>
    /// Builds the size rule, passing the bounds through as decimals so the comparison keeps the precision the attribute argument lost.
    /// </summary>
    ///
    /// <param name="mode">The comparable size validation mode.</param>
    /// <param name="value">The bound compared against, or the lower of the two when the rule takes a range.</param>
    /// <param name="maxValue">The optional maximum comparison value.</param>
    ///
    /// <returns>The comparable size validation rule.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static ComparableSizeValidationRule<TRequest, TProperty> CreateComparableSizeRule<TRequest, TProperty>(
        ComparableSizeMode mode,
        decimal value,
        decimal? maxValue = null
    ) where TRequest : class => new(mode, value, maxValue);

    /// <summary>
    /// Builds the digit-count rule, narrowing the stored decimals back to the integers this family always held.
    /// </summary>
    ///
    /// <param name="mode">The digit-count validation mode.</param>
    /// <param name="value">The digit count required, or the lower of the two when the rule takes a range.</param>
    /// <param name="maxValue">The optional maximum digit count.</param>
    ///
    /// <returns>The digit-count validation rule.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static DigitCountValidationRule<TRequest, TProperty> CreateDigitRule<TRequest, TProperty>(
        DigitMode mode,
        int value,
        int? maxValue = null
    ) where TRequest : class => new(mode, value, maxValue);

    /// <summary>
    /// Builds the upload rule from whichever of the value list or the dimension constraints the attribute supplied.
    /// </summary>
    ///
    /// <param name="mode">Which property of the upload is constrained, which also picks the message the failure reports.</param>
    /// <param name="values">The values compared against, absent for a rule whose constraint needs none.</param>
    /// <param name="dimensionConstraints">The width and height to enforce, absent for a file rule that constrains something else.</param>
    ///
    /// <returns>The file constraint validation rule.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static FileConstraintValidationRule<TRequest, TProperty> CreateFileRule<TRequest, TProperty>(
        FileConstraintMode mode,
        IReadOnlyList<string>? values = null,
        ImageDimensionConstraints? dimensionConstraints = null
    ) where TRequest : class => new(mode, values, dimensionConstraints);

    /// <summary>
    /// Builds the conditional rule, unpacking the target and condition modes stored together as a tuple.
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
    /// Builds the accepted-or-declined conditional rule, unpacking the target and state modes stored together as a tuple.
    /// </summary>
    ///
    /// <returns>The conditional state validation rule.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private IPropertyValidationRule<TRequest, TProperty> CreateConditionalStateRule<TRequest, TProperty>() where TRequest : class
    {
        (ConditionalStateTargetMode targetMode, ConditionalStateMode stateMode)
            = ((ConditionalStateTargetMode, ConditionalStateMode))_mode!;

        return new NamedConditionalStateValidationRule<TRequest, TProperty>(targetMode, stateMode, _field!);
    }

    /// <summary>
    /// Builds the multi-field presence rule, unpacking the target and trigger modes stored together as a tuple.
    /// </summary>
    ///
    /// <returns>The multi-field presence validation rule.</returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private IPropertyValidationRule<TRequest, TProperty> CreateMultiFieldPresenceRule<TRequest, TProperty>() where TRequest : class
    {
        (MultiFieldPresenceTargetMode targetMode, MultiFieldPresenceTriggerMode triggerMode)
            = ((MultiFieldPresenceTargetMode, MultiFieldPresenceTriggerMode))_mode!;

        return new NamedMultiFieldPresenceValidationRule<TRequest, TProperty>(targetMode, triggerMode, _fields!);
    }

    /// <summary>
    /// Builds the date rule, reading the stored target as a property name or a literal date according to the flag saved beside it.
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
