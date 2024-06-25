namespace CreateMoreIdeoligions;

static class Log
{
    const string Prefix = "[CreateMoreIdeoligions] ";

    public static bool OpenOnMessage
    {
        get => Verse.Log.openOnMessage;
        set => Verse.Log.openOnMessage = value;
    }

    public static void Message(string text) => Verse.Log.Message($"{Prefix}{text}");

    public static void Message(object obj) => Message(obj.ToString());

    public static void Warning(string text) => Verse.Log.Warning($"{Prefix}{text}");

    public static void WarningOnce(string text, int key) => Verse.Log.WarningOnce($"{Prefix}{text}", key);

    public static void Error(string text) => Verse.Log.Error($"{Prefix}{text}");

    public static void ErrorOnce(string text, int key) => Verse.Log.ErrorOnce($"{Prefix}{text}", key);

    public static void Clear() => Verse.Log.Clear();

    public static void TryOpenLogWindow() => Verse.Log.TryOpenLogWindow();
}
