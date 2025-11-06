namespace VisualStateSharer.Utils;

public static class Logger 
{
    private static bool _isEnabled = true;
    
    public static void Enable() => _isEnabled = true;
    public static void Disable() => _isEnabled = false;
    
    public static void Info(string message)
    {
        if (!_isEnabled) return;
        Console.WriteLine($"[INFO] {DateTime.Now:HH:mm:ss} - {message}");
    }
    
    public static void Warning(string message)
    {
        if (!_isEnabled) return;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[WARN] {DateTime.Now:HH:mm:ss} - {message}");
        Console.ResetColor();
    }
    
    public static void Error(string message, Exception? ex = null)
    {
        if (!_isEnabled) return;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[ERROR] {DateTime.Now:HH:mm:ss} - {message}");
        if (ex != null)
            Console.WriteLine($"  Exception: {ex.Message}");
        Console.ResetColor();
    }
    
    public static void Debug(string message)
    {
        if (!_isEnabled) return;
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine($"[DEBUG] {DateTime.Now:HH:mm:ss} - {message}");
        Console.ResetColor();
    }
}