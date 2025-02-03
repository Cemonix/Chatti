using System;
using Server.Enums;

namespace Server;

public class Logger<T> ()
{
    public static void LogInfo(string message) => Log(message, LogLevel.Info);
    public static void LogDebug(string message) => Log(message, LogLevel.Debug);
    public static void LogWarning(string message) => Log(message, LogLevel.Warning);
    public static void LogError(string message) => Log(message, LogLevel.Error);

    private static void Log(string message, LogLevel logLevel)
    {
        Console.ForegroundColor = logLevel switch
        {
            LogLevel.Info => ConsoleColor.White,
            LogLevel.Debug => ConsoleColor.Gray,
            LogLevel.Warning => ConsoleColor.Yellow,
            LogLevel.Error => ConsoleColor.Red,
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null),
        };
        var logMessage = $"{DateTime.Now} [{logLevel}] {message}";
        logMessage = logLevel == LogLevel.Debug ? $"{logMessage}: {typeof(T).Name}" : logMessage;
        Console.WriteLine(logMessage);
        Console.ResetColor();
    }
}
