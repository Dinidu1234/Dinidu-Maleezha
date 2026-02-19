using System.Windows;
using System.Windows.Controls;
using KickBlastStableLight.Views;

namespace KickBlastStableLight;

public partial class MainWindow : Window
{
    private readonly DashboardView _dashboardView;
    private readonly AthletesView _athletesView;
    private readonly CalculatorView _calculatorView;
    private readonly HistoryView _historyView;
    private readonly SettingsView _settingsView;

    public MainWindow(string username)
    {
        InitializeComponent();
        TxtUserChip.Text = username;

        _dashboardView = new DashboardView();
        _athletesView = new AthletesView();
        _calculatorView = new CalculatorView();
        _historyView = new HistoryView();
        _settingsView = new SettingsView();

        Loaded += (_, __) => ShowPage("Dashboard");
    }

    private void ShowPage(string page)
    {
        switch (page)
        {
            case "Dashboard": MainContent.Content = _dashboardView; break;
            case "Athletes": MainContent.Content = _athletesView; break;
            case "Calculator": MainContent.Content = _calculatorView; break;
            case "History": MainContent.Content = _historyView; break;
            case "Settings": MainContent.Content = _settingsView; break;
        }

        TxtPageTitle.Text = page;
    }

    private void BtnDash_Click(object sender, RoutedEventArgs e) => ShowPage("Dashboard");
    private void BtnAthletes_Click(object sender, RoutedEventArgs e) => ShowPage("Athletes");
    private void BtnCalc_Click(object sender, RoutedEventArgs e) => ShowPage("Calculator");
    private void BtnHistory_Click(object sender, RoutedEventArgs e) => ShowPage("History");
    private void BtnSettings_Click(object sender, RoutedEventArgs e) => ShowPage("Settings");

    private void BtnLogout_Click(object sender, RoutedEventArgs e)
    {
        var login = new LoginWindow();
        Application.Current.MainWindow = login;
        login.Show();
        Close();
    }

    private void TxtGlobalSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        var text = TxtGlobalSearch.Text.Trim();
        _athletesView.ApplyGlobalSearch(text);
    }
}
