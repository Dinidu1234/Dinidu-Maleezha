namespace KickBlastStableLight.Models;

public class Athlete
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Plan { get; set; } = "Beginner";
    public decimal Weight { get; set; }
    public decimal TargetWeight { get; set; }
    public string Notes { get; set; } = string.Empty;
}
