using System.Windows;
using System.Windows.Controls;
using KickBlastStableLight.Data;
using KickBlastStableLight.Helpers;
using KickBlastStableLight.Models;

namespace KickBlastStableLight.Views;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();
        Loaded += (_, __) => SafeLoad();
    }

    private void SafeLoad()
    {
        try
        {
            if (TxtBeginner == null) return;
            var p = Db.GetPricing();
            TxtBeginner.Text = p.Beginner.ToString();
            TxtIntermediate.Text = p.Intermediate.ToString();
            TxtElite.Text = p.Elite.ToString();
            TxtCompetition.Text = p.Competition.ToString();
            TxtCoachingRate.Text = p.CoachingRate.ToString();
        }
        catch
        {
        }
    }

    private void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        TxtInlineError.Text = string.Empty;
        if (!decimal.TryParse(TxtBeginner.Text, out var b) || !decimal.TryParse(TxtIntermediate.Text, out var i) ||
            !decimal.TryParse(TxtElite.Text, out var el) || !decimal.TryParse(TxtCompetition.Text, out var c) ||
            !decimal.TryParse(TxtCoachingRate.Text, out var r))
        {
            TxtInlineError.Text = "Enter valid numeric values.";
            ToastHelper.Show(ToastBorder, ToastText, "Validation error", true);
            return;
        }

        Db.SavePricing(new Pricing
        {
            Id = 1,
            Beginner = b,
            Intermediate = i,
            Elite = el,
            Competition = c,
            CoachingRate = r
        });

        SafeLoad();
        ToastHelper.Show(ToastBorder, ToastText, "Pricing updated");
    }
}
