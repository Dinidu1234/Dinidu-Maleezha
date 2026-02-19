using System.Windows.Controls;
using KickBlastStableLight.Data;
using KickBlastStableLight.Helpers;
using KickBlastStableLight.Models;

namespace KickBlastStableLight.Views;

public partial class HistoryView : UserControl
{
    public HistoryView()
    {
        InitializeComponent();
        Loaded += (_, __) => SafeLoad();
    }

    private void SafeLoad()
    {
        try
        {
            if (CmbAthlete == null || CmbMonth == null || TxtYear == null) return;

            CmbAthlete.Items.Clear();
            CmbAthlete.Items.Add("All");
            foreach (var a in Db.GetAthletes("", "All")) CmbAthlete.Items.Add(a.Name);
            CmbAthlete.SelectedIndex = 0;

            CmbMonth.Items.Clear();
            CmbMonth.Items.Add("All");
            for (var i = 1; i <= 12; i++) CmbMonth.Items.Add(new DateTime(2024, i, 1).ToString("MMMM"));
            CmbMonth.SelectedItem = DateTime.Now.ToString("MMMM");
            TxtYear.Text = DateTime.Now.Year.ToString();
            ReloadGrid();
        }
        catch
        {
        }
    }

    private void Filter_Changed(object sender, SelectionChangedEventArgs e) => ReloadGrid();
    private void Filter_Changed(object sender, TextChangedEventArgs e) => ReloadGrid();

    private void ReloadGrid()
    {
        var athlete = CmbAthlete.SelectedItem?.ToString() ?? "All";
        var month = CmbMonth.SelectedItem?.ToString() ?? "All";
        var year = int.TryParse(TxtYear.Text, out var y) ? y : 0;
        GridHistory.ItemsSource = Db.GetHistory(athlete, month, year);
    }

    private void GridHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (GridHistory.SelectedItem is not MonthlyCalculation m) return;
        Drawer.Visibility = System.Windows.Visibility.Visible;
        TxtDetails.Text =
            $"Athlete: {m.AthleteName}\nPlan: {m.Plan}\nTotal: {CurrencyHelper.ToLkr(m.Total)}\n" +
            $"Training: {CurrencyHelper.ToLkr(m.TrainingCost)}\nCoaching: {CurrencyHelper.ToLkr(m.CoachingCost)}\n" +
            $"Competition: {CurrencyHelper.ToLkr(m.CompetitionCost)}\nWeight: {m.WeightStatus}\n" +
            $"Next competition: {m.NextCompetitionDate}\nCreated: {m.CreatedAt}";
    }
}
