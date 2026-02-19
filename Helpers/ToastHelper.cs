using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace KickBlastStableLight.Helpers;

public static class ToastHelper
{
    public static void Show(Border toastBorder, TextBlock toastText, string message, bool isError = false)
    {
        if (toastBorder == null || toastText == null)
        {
            return;
        }

        toastText.Text = message;
        toastBorder.Background = isError
            ? new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(220, 38, 38))
            : new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(37, 99, 235));
        toastBorder.Visibility = Visibility.Visible;

        var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
        timer.Tick += (_, __) =>
        {
            toastBorder.Visibility = Visibility.Collapsed;
            timer.Stop();
        };
        timer.Start();
    }
}
