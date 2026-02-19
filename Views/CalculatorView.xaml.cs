using System.Windows.Controls;
using KickBlastStableLight.Data;
using KickBlastStableLight.Helpers;
using KickBlastStableLight.Models;

namespace KickBlastStableLight.Views;

public partial class CalculatorView : UserControl
{
    private List<Athlete> _athletes = new();

    public CalculatorView()
    {
        InitializeComponent();
        Loaded += (_, __) => SafeLoad();
    }

    private void SafeLoad()
    {
        try
        {
            if (CmbAthlete == null) return;
            _athletes = Db.GetAthletes(string.Empty, "All");
            CmbAthlete.ItemsSource = _athletes;
            CmbAthlete.DisplayMemberPath = "Name";
            CmbAthlete.SelectedValuePath = "Id";
            if (_athletes.Count > 0) CmbAthlete.SelectedIndex = 0;
        }
        catch
        {
        }
    }

    private void CmbAthlete_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CmbAthlete.SelectedItem is Athlete a && a.Plan == "Beginner")
        {
            TxtCompetitions.Text = "0";
            TxtBeginnerNote.Text = "Beginner plan: competitions are forced to 0.";
        }
        else
        {
            TxtBeginnerNote.Text = string.Empty;
        }
    }

    private void BtnCalculate_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        TxtInlineError.Text = string.Empty;
        if (CmbAthlete.SelectedItem is not Athlete athlete)
        {
            TxtInlineError.Text = "Select an athlete.";
            return;
        }
        if (!ValidationHelper.IsNonNegativeInt(TxtCompetitions.Text) || !ValidationHelper.IsRangeInt(TxtCoachingHours.Text, 0, 5))
        {
            TxtInlineError.Text = "Competitions >=0 and Coaching hours must be 0..5.";
            ToastHelper.Show(ToastBorder, ToastText, "Validation error", true);
            return;
        }

        var competitions = int.Parse(TxtCompetitions.Text);
        if (athlete.Plan == "Beginner") competitions = 0;
        var coachingHours = int.Parse(TxtCoachingHours.Text);

        var pricing = Db.GetPricing();
        var weeklyFee = athlete.Plan switch
        {
            "Intermediate" => pricing.Intermediate,
            "Elite" => pricing.Elite,
            _ => pricing.Beginner
        };

        var training = weeklyFee * 4;
        var coaching = coachingHours * 4 * pricing.CoachingRate;
        var competition = athlete.Plan is "Intermediate" or "Elite" ? competitions * pricing.Competition : 0;
        var total = training + coaching + competition;

        var status = athlete.Weight > athlete.TargetWeight ? "Over" : athlete.Weight < athlete.TargetWeight ? "Under" : "On target";
        var secondSat = DateHelper.GetSecondSaturday(DateTime.Now);

        TxtResult.Text =
            $"Training: {CurrencyHelper.ToLkr(training)}\n" +
            $"Coaching: {CurrencyHelper.ToLkr(coaching)}\n" +
            $"Competition: {CurrencyHelper.ToLkr(competition)}\n" +
            $"Total: {CurrencyHelper.ToLkr(total)}\n" +
            $"Weight status: {status}\n" +
            $"Next competition: {secondSat:dd MMM yyyy}";

        Db.SaveCalculation(new MonthlyCalculation
        {
            AthleteId = athlete.Id,
            AthleteName = athlete.Name,
            Plan = athlete.Plan,
            Competitions = competitions,
            CoachingHours = coachingHours,
            TrainingCost = training,
            CoachingCost = coaching,
            CompetitionCost = competition,
            Total = total,
            WeightStatus = status,
            MonthName = DateTime.Now.ToString("MMMM"),
            Year = DateTime.Now.Year,
            NextCompetitionDate = secondSat.ToString("dd MMM yyyy")
        });

        ToastHelper.Show(ToastBorder, ToastText, "Calculation saved");
    }
}
