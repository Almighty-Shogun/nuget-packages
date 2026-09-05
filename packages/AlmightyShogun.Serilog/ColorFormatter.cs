using Serilog.Events;
using Serilog.Parsing;
using Serilog.Formatting;
using System.Globalization;

namespace AlmightyShogun.Serilog;

/// <summary>
/// Renders a Serilog event as one console line: a colored level and timestamp prefix, then the message template with each
/// property colored by its type or by an explicit shorthand, and any exception appended below in dark gray.
/// </summary>
///
/// <param name="enableColors">
/// Whether escape codes are written at all. When <c>false</c> the same text is produced without them, so a redirected log
/// stays readable rather than filling with escape sequences.
/// </param>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
internal sealed class ColorFormatter(bool enableColors) : ITextFormatter
{
    /// <summary>
    /// Gets whether escape codes should be written by default: <c>true</c> when the process output is not redirected and
    /// <c>NO_COLOR</c> is unset or set to an empty string. Nothing here tests what the receiving terminal can render.
    /// </summary>
    ///
    /// <remarks>
    /// Evaluated once per process, so a console redirected after start is not noticed. This is the default the registration
    /// helpers fall back to when the caller expresses no preference.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    internal static bool OutputSupportsColors { get; } =
        !Console.IsOutputRedirected && string.IsNullOrEmpty(Environment.GetEnvironmentVariable("NO_COLOR"));

    /// <summary>
    /// Writes one event, then a trailing newline. A property named in the template but absent from the event is written back
    /// as <c>{Name}</c>, or <c>{Name:format}</c> where the token carried a format, so a template typo is visible in the log
    /// instead of silent. Alignment and destructuring hints are not carried through, so the token is recognisable rather
    /// than identical to what the template held.
    /// </summary>
    ///
    /// <param name="logEvent">The event to render, supplying the level, timestamp, template, properties, and exception.</param>
    /// <param name="output">The writer receiving the line. Not flushed here.</param>
    ///
    /// <remarks>
    /// A property format specifier may carry a color after a <c>|</c>, as in <c>{Count:N0|c}</c>, where the left side is the
    /// numeric format and the right side is a shorthand from <see cref="AnsiColor"/>. Without a <c>|</c> at all, the color
    /// follows the
    /// value's type.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public void Format(LogEvent logEvent, TextWriter output)
    {
        MessageTemplate messageTemplate = logEvent.MessageTemplate;
        IReadOnlyDictionary<string, LogEventPropertyValue> properties = logEvent.Properties;

        Write(output, GetLogLevelColor(logEvent.Level));
        output.Write($"[{logEvent.Timestamp:HH:mm:ss} {logEvent.Level.ToString()[..3].ToUpperInvariant()}] ");
        Write(output, AnsiColor.Reset);

        foreach (MessageTemplateToken token in messageTemplate.Tokens)
            switch (token)
            {
                case TextToken textToken:
                    output.Write(textToken.Text);
                    break;
                case PropertyToken propToken:
                {
                    string format = propToken.Format ?? "";
                    string propName = propToken.PropertyName;

                    string? colorSpec = null;
                    string numericFormat = format;

                    if (format.Contains('|'))
                    {
                        string[] parts = format.Split('|', 2);

                        numericFormat = parts[0];
                        colorSpec = parts[1];
                    }

                    if (!properties.TryGetValue(propName, out LogEventPropertyValue? propertyValue))
                    {
                        output.Write("{");
                        output.Write(propName);

                        if (!string.IsNullOrEmpty(format))
                        {
                            output.Write(":");
                            output.Write(format);
                        }

                        output.Write("}");
                        continue;
                    }

                    string renderedValue = RenderPropertyValue(propertyValue, numericFormat);

                    string ansiColor = colorSpec is not null ? AnsiColor.FromShort(colorSpec) : GetDefaultColor(propertyValue);

                    Write(output, ansiColor);
                    output.Write(renderedValue);
                    Write(output, AnsiColor.Reset);

                    break;
                }
            }

        if (logEvent.Exception is not null)
        {
            output.WriteLine();
            Write(output, AnsiColor.DarkGray);
            output.Write(logEvent.Exception);
            Write(output, AnsiColor.Reset);
        }

        output.WriteLine();
    }

    /// <summary>
    /// Turns a property value into the text that appears in the line.
    /// </summary>
    ///
    /// <param name="value">
    /// The value to render. A scalar is written directly; anything structured falls back to Serilog's own rendering.
    /// </param>
    /// <param name="numericFormat">
    /// The format applied when the scalar's own value implements <see cref="IFormattable"/>, under
    /// <see cref="CultureInfo.InvariantCulture"/>. Ignored when empty, when that value is not formattable, and for a
    /// structured value, which Serilog renders with the default format provider instead.
    /// </param>
    ///
    /// <returns>The rendered text, or the literal <c>null</c> for a scalar holding no value.</returns>
    ///
    /// <remarks>
    /// A format string the value rejects with a <c>FormatException</c> is swallowed and the unformatted value is written
    /// instead; anything else it throws escapes. Only the formatted path uses <see cref="CultureInfo.InvariantCulture"/>: an
    /// empty format, a value that is not formattable, and that swallowed failure all fall back to
    /// <see cref="object.ToString"/>.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private static string RenderPropertyValue(LogEventPropertyValue value, string? numericFormat)
    {
        if (value is ScalarValue scalar)
        {
            object? obj = scalar.Value;

            if (obj == null)
                return "null";

            if (string.IsNullOrEmpty(numericFormat) || obj is not IFormattable formattable)
                return obj.ToString() ?? string.Empty;

            try
            {
                return formattable.ToString(numericFormat, CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
                return obj.ToString() ?? string.Empty;
            }
        }

        using var stringWriter = new StringWriter();
        value.Render(stringWriter);

        return stringWriter.ToString();
    }

    /// <summary>
    /// Writes an escape code, or nothing when colors are off.
    /// </summary>
    ///
    /// <param name="output">The writer receiving the escape code.</param>
    /// <param name="ansiColor">The escape code to write, discarded when colors are off.</param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private void Write(TextWriter output, string ansiColor)
    {
        if (enableColors)
            output.Write(ansiColor);
    }

    /// <summary>
    /// Picks a color from the value's type, used whenever the template's format carries no <c>|</c>. A format ending in one
    /// selects white instead, since the empty shorthand matches nothing.
    /// </summary>
    ///
    /// <param name="value">The value whose runtime type decides the color.</param>
    ///
    /// <returns>
    /// Cyan for numeric types, magenta for <see cref="bool"/>, dark gray for null, and white for strings and anything else,
    /// including structured values.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private static string GetDefaultColor(LogEventPropertyValue value)
    {
        if (value is not ScalarValue scalar)
            return AnsiColor.White;

        object? obj = scalar.Value;

        if (obj == null)
            return AnsiColor.DarkGray;

        return obj switch
        {
            string => AnsiColor.White,
            byte or sbyte or short or ushort or int or uint or long or ulong or nint or nuint => AnsiColor.Cyan,
            float or double or decimal or Half => AnsiColor.Cyan,
            bool => AnsiColor.Magenta,
            _ => AnsiColor.White
        };
    }

    /// <summary>
    /// Picks the color of the level and timestamp prefix.
    /// </summary>
    ///
    /// <param name="logLevel">The level being written.</param>
    ///
    /// <returns>
    /// Green for <c>Information</c>, yellow for <c>Warning</c>, red for <c>Error</c>, bright red for <c>Fatal</c>, and white
    /// for <c>Verbose</c> and <c>Debug</c>.
    /// </returns>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    private static string GetLogLevelColor(LogEventLevel logLevel) => logLevel switch
    {
        LogEventLevel.Verbose => AnsiColor.White,
        LogEventLevel.Debug => AnsiColor.White,
        LogEventLevel.Information => AnsiColor.Green,
        LogEventLevel.Warning => AnsiColor.Yellow,
        LogEventLevel.Error => AnsiColor.Red,
        LogEventLevel.Fatal => AnsiColor.BrightRed,
        _ => AnsiColor.White
    };
}
