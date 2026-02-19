using System.Windows.Controls;
using KickBlastStableLight.Data;

namespace KickBlastStableLight.Views;

public partial class DashboardView : UserControl
{
    public DashboardView()
    {
        InitializeComponent();
        Loaded += (_, __) => SafeLoad();
    }

    private void SafeLoad()
    {
        try
        {
            if (TxtAthletes == null || GridRecent == null) return;
            var s = Db.GetDashboardStats();
            TxtAthletes.Text = s["Athletes"];
            TxtCalcs.Text = s["Calculations"];
            TxtRevenue.Text = s["Revenue"];
            TxtNextComp.Text = s["NextCompetition"];
            GridRecent.ItemsSource = Db.GetRecentCalculations();
        }
        catch
        {
        }
    }
}
