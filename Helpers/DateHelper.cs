namespace KickBlastStableLight.Helpers;

public static class DateHelper
{
    public static DateTime GetSecondSaturday(DateTime date)
    {
        var first = new DateTime(date.Year, date.Month, 1);
        var offset = ((int)DayOfWeek.Saturday - (int)first.DayOfWeek + 7) % 7;
        var firstSaturday = first.AddDays(offset);
        return firstSaturday.AddDays(7);
    }
}
