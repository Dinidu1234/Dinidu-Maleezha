namespace KickBlastStableLight.Models;

public class Pricing
{
    public int Id { get; set; } = 1;
    public decimal Beginner { get; set; } = 2400;
    public decimal Intermediate { get; set; } = 3400;
    public decimal Elite { get; set; } = 5000;
    public decimal Competition { get; set; } = 1700;
    public decimal CoachingRate { get; set; } = 1300;
}
