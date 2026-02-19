using System.Windows;
using KickBlastStableLight.Data;
using KickBlastStableLight.Views;

namespace KickBlastStableLight;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        Db.Init();

        var login = new LoginWindow();
        MainWindow = login;
        login.Show();
    }
}
