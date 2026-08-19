namespace AlmightyShogun.Utils;

/// <summary>
/// Groups the console primitives a long-running command-line application needs: naming the window and taking over what
/// <c>Ctrl+C</c> means. Every member is static, the type is never registered in a container, and nothing here has an
/// effect on a process whose output is redirected rather than attached to a terminal.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
public static class ConsoleUtils
{
    /// <summary>
    /// Whether the cancellation handler has been attached, as an <see cref="int"/> so it can be flipped and tested in
    /// one atomic step. <c>0</c> means no handler is attached, <c>1</c> means one is.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static int _cancellationPrevented;

    /// <summary>
    /// Sets the console window title. Behavior is platform and terminal dependent: a terminal that does not support titles
    /// discards the value rather than reporting an error, so this is presentation only and never worth branching on.
    /// </summary>
    ///
    /// <param name="title">
    /// The text to show as the window title. Passed through unchanged, so any truncation or escaping is the terminal's.
    /// </param>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public static void Title(string title) => Console.Title = title;

    /// <summary>
    /// Erases the line above the cursor and parks the cursor at its start, so a prompt or a progress line can be
    /// replaced instead of scrolling away. Does nothing when output is redirected or the cursor is already at the top.
    /// </summary>
    ///
    /// <remarks>
    /// A host can refuse cursor movement even when output is not reported as redirected, so the move is attempted and
    /// the resulting failure swallowed. Erasing a line is cosmetic, and a console helper must not take down a process
    /// over it.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    public static void RemoveLastLine()
    {
        if (Console.IsOutputRedirected || Console.CursorTop <= 0) return;

        try
        {
            int line = Console.CursorTop - 1;

            Console.SetCursorPosition(0, line);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, line);
        }
        catch (Exception exception) when (exception is IOException or ArgumentOutOfRangeException or InvalidOperationException) { }
    }

    /// <summary>
    /// Prompts on the console and waits for an answer, repeating the prompt until one is available. The prompt line is
    /// erased once answered, so a sequence of questions does not fill the screen with what was already asked.
    /// </summary>
    ///
    /// <param name="question">
    /// The text shown after a <c>[QUESTION]</c> marker. Written without a trailing newline, so the answer is typed on
    /// the same line.
    /// </param>
    /// <param name="defaultValue">
    /// The answer to use when the reader submits an empty line. Leave it unset to make the question mandatory: an empty
    /// line then re-asks rather than returning, and the loop only ends once something is typed.
    /// </param>
    ///
    /// <returns>
    /// The typed answer, or <paramref name="defaultValue"/> when the line was empty. Never <c>null</c> and never empty.
    /// </returns>
    ///
    /// <remarks>
    /// Typed input is colored, and the color is reset before returning even though the console is left on whatever
    /// line the caller was on. A redirected input stream returns <c>null</c> from every read, which with no default is
    /// an infinite loop, so a mandatory question belongs only in an interactive tool.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public static async Task<string> AskQuestionAsync(string question, string? defaultValue = null)
    {
        string? answer = null;

        while (answer is null)
        {
            await Console.Out.WriteAsync($"[QUESTION] {question}: ");
            Console.ForegroundColor = ConsoleColor.Blue;

            string input = Console.ReadLine() ?? "";

            answer = input.Length >= 1 ? input : defaultValue;

            Console.ResetColor();
            RemoveLastLine();
        }

        return answer;
    }

    /// <summary>
    /// Stops <c>Ctrl+C</c> from terminating the process, so a long-running console application can decide for itself when to
    /// shut down. Safe to call from any thread and any number of times; only the first call attaches a handler.
    /// </summary>
    ///
    /// <remarks>
    /// There is no counterpart that restores the default. Once suppressed, cancellation stays suppressed for the life of the
    /// process, and the application must expose some other way to stop. A hosted application should prefer
    /// <c>UseCustomConsoleLifetime</c> from <c>AlmightyShogun.Hosting.Utils</c>, which suppresses the same key press but still
    /// shuts down cleanly on <c>SIGTERM</c>.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.1.0</since>
    public static void PreventCancellation()
    {
        if (Interlocked.Exchange(ref _cancellationPrevented, 1) != 0) return;

        Console.CancelKeyPress += (_, e) => e.Cancel = true;
    }
}
