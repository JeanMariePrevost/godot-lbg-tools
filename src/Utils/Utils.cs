using Godot;
using System.IO;
using System.Diagnostics;

namespace LBG.LBGTools.Utils;

/// <summary>
/// A general helper class for various non-specific utility functions.
/// </summary>
public static class UtilsGeneral {
    /// <summary>
    /// Prints a message to Godot's output panel with source location information.
    /// </summary>
    public static void Print(object message) {
        var msgText = message?.ToString() ?? "<null>";

        var sf = new StackTrace(1, true).GetFrame(0);
        var filename = Path.GetFileName(sf?.GetFileName() ?? "");
        var lineNumber = sf?.GetFileLineNumber();
        var callerInfo = $"[color=#909090][{filename}:{lineNumber}][/color] ";

        var fullMessage = $"{callerInfo}[color=#B0E0E6]{msgText}[/color]";
        GD.PrintRich(fullMessage);
    }

    /// <summary>
    /// Prints a warning message to Godot's output panel with source location information.
    /// The message is prefixed with a warning symbol and displayed in orange.
    /// </summary>
    public static void PrintWarning(object message) {
        var msgText = message?.ToString() ?? "<null>";

        var sf = new StackTrace(1, true).GetFrame(0);
        var filename = Path.GetFileName(sf?.GetFileName() ?? "");
        var lineNumber = sf?.GetFileLineNumber();
        var callerInfo = $"[color=#909090][{filename}:{lineNumber}][/color] ";

        var fullMessage = $"[color=#FFD700][WARNING][/color] {callerInfo}[color=#FFD700]{msgText}[/color]";
        GD.PrintRich(fullMessage);
    }

    /// <summary>
    /// Prints an error message to Godot's output panel with source location information.
    /// The message is prefixed with an error symbol and displayed in red.
    /// </summary>
    public static void PrintError(object message) {
        var msgText = message?.ToString() ?? "<null>";

        var sf = new StackTrace(1, true).GetFrame(0);
        var filename = Path.GetFileName(sf?.GetFileName() ?? "");
        var lineNumber = sf?.GetFileLineNumber();
        var callerInfo = $"[color=#909090][{filename}:{lineNumber}][/color] ";

        var fullMessage = $"[color=#FF0000][b][ERROR][/b][/color] {callerInfo}[color=#FF0000][b]{msgText}[/b][/color]";
        GD.PrintRich(fullMessage);
    }
}

