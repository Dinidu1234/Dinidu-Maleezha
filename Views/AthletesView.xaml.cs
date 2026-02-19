using System.Windows;
using System.Windows.Controls;
using KickBlastStableLight.Data;
using KickBlastStableLight.Helpers;
using KickBlastStableLight.Models;

namespace KickBlastStableLight.Views;

public partial class AthletesView : UserControl
{
    private int _currentId;

    public AthletesView()
    {
        InitializeComponent();
        Loaded += (_, __) => SafeLoad();
    }

    public void ApplyGlobalSearch(string search)
    {
        if (TxtSearch != null)
        {
            TxtSearch.Text = search;
        }
    }

    private void SafeLoad()
    {
        try
        {
            if (GridAthletes == null || CmbPlanFilter == null) return;
            ReloadGrid();
        }
        catch
        {
        }
    }

    private void ReloadGrid()
    {
        var plan = (CmbPlanFilter.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "All";
        GridAthletes.ItemsSource = Db.GetAthletes(TxtSearch.Text.Trim(), plan);
    }

    private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e) => ReloadGrid();
    private void CmbPlanFilter_SelectionChanged(object sender, SelectionChangedEventArgs e) => ReloadGrid();
    private void BtnFab_Click(object sender, RoutedEventArgs e) { Drawer.Visibility = Visibility.Visible; ClearForm(); }

    private void GridAthletes_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (GridAthletes.SelectedItem is not Athlete a) return;
        Drawer.Visibility = Visibility.Visible;
        _currentId = a.Id;
        TxtName.Text = a.Name;
        TxtAge.Text = a.Age.ToString();
        CmbPlan.SelectedIndex = a.Plan switch { "Intermediate" => 1, "Elite" => 2, _ => 0 };
        TxtWeight.Text = a.Weight.ToString();
        TxtTargetWeight.Text = a.TargetWeight.ToString();
        TxtNotes.Text = a.Notes;
    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        TxtInlineError.Text = string.Empty;
        if (ValidationHelper.IsEmpty(TxtName.Text) || !int.TryParse(TxtAge.Text, out var age) ||
            !decimal.TryParse(TxtWeight.Text, out var w) || !decimal.TryParse(TxtTargetWeight.Text, out var t))
        {
            TxtInlineError.Text = "Please fill valid Name, Age, Weight, Target Weight.";
            ToastHelper.Show(ToastBorder, ToastText, "Validation error", true);
            return;
        }

        var athlete = new Athlete
        {
            Id = _currentId,
            Name = TxtName.Text.Trim(),
            Age = age,
            Plan = (CmbPlan.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Beginner",
            Weight = w,
            TargetWeight = t,
            Notes = TxtNotes.Text.Trim()
        };

        Db.UpsertAthlete(athlete);
        ReloadGrid();
        ToastHelper.Show(ToastBorder, ToastText, "Athlete saved");
    }

    private void BtnDelete_Click(object sender, RoutedEventArgs e)
    {
        if (_currentId <= 0) return;
        if (MessageBox.Show("Delete this athlete?", "Confirm", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;
        Db.DeleteAthlete(_currentId);
        ReloadGrid();
        ClearForm();
        ToastHelper.Show(ToastBorder, ToastText, "Athlete deleted");
    }

    private void BtnClear_Click(object sender, RoutedEventArgs e) => ClearForm();

    private void ClearForm()
    {
        _currentId = 0;
        TxtName.Text = string.Empty;
        TxtAge.Text = string.Empty;
        CmbPlan.SelectedIndex = 0;
        TxtWeight.Text = string.Empty;
        TxtTargetWeight.Text = string.Empty;
        TxtNotes.Text = string.Empty;
        TxtInlineError.Text = string.Empty;
    }
}
