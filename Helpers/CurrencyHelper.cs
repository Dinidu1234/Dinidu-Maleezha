namespace KickBlastStableLight.Helpers;

public static class CurrencyHelper
{
    public static string ToLkr(decimal value)
    {
        return $"LKR {value:N2}";
    }
}
