namespace AlmightyShogun.Utils;

/// <summary>
/// Groups the console primitives a long-running command-line application needs: naming the window and taking over what
/// <c>Ctrl+C</c> means. Every member is static and the type is never registered in a container. Only the cursor rewrite
/// checks for redirected output and skips itself; the prompt still writes to a redirected stream, so its text appears in
/// the captured output while the coloring around it does not.
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
    /// Prompts on the console and waits for an answer, repeating the prompt until one is available or the input stream ends.
    /// The prompt line is erased once answered, so a sequence of questions does not fill the screen with what was already asked.
    /// </summary>
    ///
    /// <param name="question">
    /// The text shown after a <c>[QUESTION]</c> marker. Written without a trailing newline, so the answer is typed on
    /// the same line.
    /// </param>
    /// <param name="defaultValue">
    /// The answer to use when the reader submits an empty line or the input stream ends. Leave it unset to make the question
    /// mandatory: an empty line then re-asks rather than returning, and only a typed answer or a closed stream ends the loop.
    /// </param>
    /// <param name="cancellationToken">
    /// Abandons the question. Only observed between reads, so it takes effect once the pending line is submitted rather than
    /// interrupting a reader who is part way through typing one.
    /// </param>
    ///
    /// <returns>
    /// The typed answer, <paramref name="defaultValue"/> when the line was empty or the input stream ended, or <c>null</c>
    /// when the stream ended on a mandatory question. Never empty.
    /// </returns>
    ///
    /// <exception cref="OperationCanceledException"><paramref name="cancellationToken"/> was signaled before a read began.</exception>
    ///
    /// <remarks>
    /// Typed input is colored, and the color is reset before returning even though the console is left on whatever
    /// line the caller was on.
    ///
    /// A closed or exhausted input stream ends the loop rather than spinning on it, which is what a redirected process gets on
    /// every read. That is the only path to a <c>null</c> result, so a caller that always passes a default never sees one.
    ///
    /// <see cref="Console.In"/> reads synchronously whatever is asked of it, so awaiting here yields no thread back while a
    /// reader is typing. The asynchronous shape is for composing with asynchronous callers, not for scaling.
    /// </remarks>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public static async Task<string?> AskQuestionAsync(
        string question,
        string? defaultValue = null,
        CancellationToken cancellationToken = default
    )
    {
        while (true)
        {
            await Console.Out.WriteAsync($"[QUESTION] {question}: ");
            Console.ForegroundColor = ConsoleColor.Blue;

            try
            {
                string? input = await Console.In.ReadLineAsync(cancellationToken);

                if (input is null) return defaultValue;

                if (input.Length >= 1) return input;

                if (defaultValue is not null) return defaultValue;
            }
            finally
            {
                Console.ResetColor();
                RemoveLastLine();
            }
        }
    }

    /// <summary>
    /// Stops <c>Ctrl+C</c> from terminating the process, so a long-running console application can decide for itself when to
    /// shut down. Safe to call from any thread and any number of times; only the first call attaches a handler.
    /// </summary>
    ///
    /// <remarks>
    /// There is no counterpart that restores the default. Once suppressed, cancellation stays suppressed for the life of the
    /// process, and the application must expose some other way to stop. A hosted application should prefer
    /// <c>UseCustomConsoleLifetime</c> from <c>AlmightyShogun.Hosting.ConsoleLifetime</c>, which suppresses the same key press but still
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
