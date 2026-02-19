namespace KickBlastStableLight.Helpers;

public static class ValidationHelper
{
    public static bool IsEmpty(string? value)
    {
        return string.IsNullOrWhiteSpace(value);
    }

    public static bool IsNonNegativeInt(string? value)
    {
        return int.TryParse(value, out var n) && n >= 0;
    }

    public static bool IsRangeInt(string? value, int min, int max)
    {
        return int.TryParse(value, out var n) && n >= min && n <= max;
    }
}
