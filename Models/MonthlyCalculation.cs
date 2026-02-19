namespace KickBlastStableLight.Models;

public class MonthlyCalculation
{
    public int Id { get; set; }
    public int AthleteId { get; set; }
    public string AthleteName { get; set; } = string.Empty;
    public string Plan { get; set; } = string.Empty;
    public int Competitions { get; set; }
    public int CoachingHours { get; set; }
    public decimal TrainingCost { get; set; }
    public decimal CoachingCost { get; set; }
    public decimal CompetitionCost { get; set; }
    public decimal Total { get; set; }
    public string WeightStatus { get; set; } = string.Empty;
    public string MonthName { get; set; } = string.Empty;
    public int Year { get; set; }
    public string NextCompetitionDate { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
}
