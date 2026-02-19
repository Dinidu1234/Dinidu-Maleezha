using System.Windows;
using KickBlastStableLight.Data;

namespace KickBlastStableLight.Views;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
        Loaded += (_, __) => TxtPassword.Password = "123456";
    }

    private void BtnLogin_Click(object sender, RoutedEventArgs e)
    {
        TxtError.Text = string.Empty;
        var user = TxtUsername.Text.Trim();
        var pass = TxtPassword.Password.Trim();

        if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(pass))
        {
            TxtError.Text = "Please enter username and password.";
            return;
        }

        if (!Db.ValidateLogin(user, pass))
        {
            TxtError.Text = "Invalid login. Use dinidu / 123456.";
            return;
        }

        var main = new MainWindow(user);
        Application.Current.MainWindow = main;
        main.Show();
        Close();
    }
}
