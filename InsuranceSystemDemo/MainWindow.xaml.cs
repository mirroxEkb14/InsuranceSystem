using InsuranceSystemDemo.Views;
using System.Windows;
using System.Windows.Threading;

namespace InsuranceSystemDemo;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        StartTransitionTimer();
    }

    #region Animation Methods
    //
    // Summary:
    //     Adjusts the delay time.
    //     Stops the timer to prevent multiple triggers.
    //     Opens the «LoginWindow».
    //     Close the «MainWindow».
    private void StartTransitionTimer()
    {
        var timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1.5)
        };
        timer.Tick += (s, e) =>
        {
            timer.Stop();

            var loginWindow = new LoginView();
            loginWindow.Show();

            this.Close();
        };
        timer.Start();
    }
    #endregion
}