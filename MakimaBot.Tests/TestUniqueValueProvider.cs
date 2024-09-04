namespace MakimaBot.Tests;

public static class TestUniqueValueProvider
{
    private static int _intCounter = 1;
    private static long _longCounter = 1;
    private static int _stringCounter = 1;

    public static int GetNextInt() => _intCounter++;

    public static long GetNextLong() => _longCounter++;

    public static string GetNextString(string prefix = "Test") => $"{prefix}{_stringCounter++}";
}
