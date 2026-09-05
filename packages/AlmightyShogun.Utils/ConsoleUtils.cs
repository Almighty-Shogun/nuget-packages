namespace AlmightyShogun.Utils;

/// <summary>
/// Groups the console primitives a long-running command-line application needs: naming the window, rewriting the last
/// line, asking a question, and taking over what the cancel key press means. Every member is static and the type is never
/// registered in a container. Only the cursor rewrite checks for redirected output and skips itself; the prompt still
/// writes to a redirected stream, so its text appears in the captured output.
/// </summary>
///
/// <author>Almighty-Shogun</author>
/// <since>1.0.0</since>
public static class ConsoleUtils
{
    /// <summary>
    /// Whether a caller has claimed the attach, as an <see cref="int"/> so it can be flipped and tested in one atomic step.
    /// <c>0</c> means no call has claimed it, <c>1</c> means one has. The subscription happens just after the flip, so a
    /// second thread can return while it is still being made.
    /// </summary>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>Unreleased</since>
    private static int _cancellationPrevented;

    /// <summary>
    /// Assigns <paramref name="title"/> straight to <see cref="Console.Title"/>. Nothing is validated, trimmed, or caught
    /// here, so every constraint that setter documents reaches the caller as an exception.
    /// </summary>
    ///
    /// <param name="title">
    /// The text to show as the window title. Passed through unchanged, so any truncation or escaping is the terminal's.
    /// </param>
    ///
    /// <exception cref="ArgumentNullException"><paramref name="title"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="title"/> is longer than 24500 characters.</exception>
    /// <exception cref="IOException">An I/O error occurred while setting the title.</exception>
    ///
    /// <author>Almighty-Shogun</author>
    /// <since>1.0.0</since>
    public static void Title(string title) => Console.Title = title;

    /// <summary>
    /// Erases the line above the cursor and parks the cursor at its start, so a prompt or a progress line can be
    /// replaced instead of scrolling away. Does nothing when output is redirected or the cursor is already at the top.
    /// </summary>
    ///
    /// <exception cref="IOException">
    /// Reading <see cref="Console.CursorTop"/> for the guard failed. That read sits outside the <c>try</c>, so unlike the
    /// same failure during the rewrite it is not swallowed.
    /// </exception>
    /// <exception cref="System.Security.SecurityException">
    /// The process is not permitted to read <see cref="Console.CursorTop"/> or to move the cursor. The <c>catch</c> filter
    /// does not list it, so it escapes from the rewrite as well as from the guard.
    /// </exception>
    ///
    /// <remarks>
    /// A host can refuse cursor movement even when output is not reported as redirected, so the move is attempted and an
    /// <see cref="IOException"/>, <see cref="ArgumentOutOfRangeException"/>, or <see cref="InvalidOperationException"/>
    /// raised by it swallowed. Everything else reaches the caller, so this can end a process, including through the
    /// <c>finally</c> in <see cref="AskQuestionAsync"/>.
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
    /// The prompt line is erased once answered when the console echoed the newline, so a sequence of questions does not
    /// fill the screen with what was already asked.
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
    /// Abandons the question. Handed to every read, so a signal raised before one begins ends the loop. Whether a read
    /// already waiting on a line is interrupted is the reader's own behavior rather than anything arranged here.
    /// </param>
    ///
    /// <returns>
    /// The typed answer, <paramref name="defaultValue"/> when the line was empty or the input stream ended, or <c>null</c>
    /// when the stream ended on a mandatory question. Empty only when that is what <paramref name="defaultValue"/> holds,
    /// since a typed answer is never the empty string.
    /// </returns>
    ///
    /// <exception cref="OperationCanceledException">
    /// <paramref name="cancellationToken"/> was signaled.
    /// </exception>
    /// <exception cref="IOException">
    /// Setting the input color, resetting it, or erasing the prompt line through <see cref="RemoveLastLine"/> failed.
    /// </exception>
    /// <exception cref="System.Security.SecurityException">
    /// The process is not permitted to change the console color or to move the cursor.
    /// </exception>
    ///
    /// <remarks>
    /// Typed input is colored, and the color is reset before returning even though the console is left on whatever
    /// line the caller was on.
    ///
    /// A closed or exhausted input stream ends the loop rather than spinning on it, because
    /// <see cref="TextReader.ReadLineAsync(CancellationToken)"/> returns <c>null</c> only once every character has been read.
    /// Redirected input is read like any other, so a piped process gets its lines and reaches that state when the pipe runs
    /// dry. That is the only path to a <c>null</c> result, so a caller that always passes a default never sees one.
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
    /// Stops <c>Ctrl+C</c> and <c>Ctrl+Break</c> from terminating the process, so a long-running console application can decide
    /// for itself when to shut down. The handler cancels every <see cref="Console.CancelKeyPress"/> without inspecting which
    /// key raised it. Safe to call from any thread and any number of times; only the first call attaches a handler.
    /// </summary>
    ///
    /// <remarks>
    /// There is no counterpart that restores the default. Once suppressed, cancellation stays suppressed for the life of the
    /// process, and the application must expose some other way to stop. A hosted application should prefer
    /// <c>UseCustomConsoleLifetime</c> from <c>AlmightyShogun.Hosting.ConsoleLifetime</c>, whose handler suppresses the same key
    /// presses unless <c>DOTNET_RUNNING_IN_IDE</c> is set, and which requests a clean shutdown on <c>SIGTERM</c> everywhere
    /// except Windows.
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
