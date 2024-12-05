#region Imports
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InsuranceSystemDemo.Controls;
using InsuranceSystemDemo.Database;
using InsuranceSystemDemo.Models;
using InsuranceSystemDemo.Utils;
using InsuranceSystemDemo.Views;
using Microsoft.EntityFrameworkCore;
using System.Windows;
#endregion

namespace InsuranceSystemDemo.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    [ObservableProperty] private string? _username;
    [ObservableProperty] private string? _password;

    public LoginViewModel()
    {
        Username = MessageContainer.LoginUsernameHint;
        //AttachConsole();
    }

    #region Console Window Logic
    [System.Runtime.InteropServices.DllImport("kernel32.dll")]
    private static extern bool AllocConsole();

    private void AttachConsole() =>
        AllocConsole();
    #endregion

    [RelayCommand]
    public void Login()
    {
        if (!ValidateInputs())
            return;

        var dbContextOptions = DatabaseContextGetter.GetDatabaseContextOptions();
        try
        {
            using var context = new DatabaseContext(dbContextOptions);
            var user = GetUser(context);

            if (!ValidateUser(user))
                return;

            OpenAppropriateWindow(user!);
        }
        catch (Exception ex)
        {
            HandleException(ex);
        }
    }

    [RelayCommand]
    public void Register()
    {
        var currentWindow = Application.Current.Windows
            .OfType<Window>()
            .SingleOrDefault(w => w.DataContext == this);

        var registerWindow = new RegisterView();
        registerWindow.Show();

        currentWindow?.Close();
    }

    #region Private Helper Methods
    private bool ValidateInputs()
    {
        if (string.IsNullOrEmpty(Username))
        {
            MessageBoxDisplayer.ShowError(MessageContainer.LoginUsernameErrorMessage);
            return false;
        }
        if (string.IsNullOrEmpty(Password))
        {
            MessageBoxDisplayer.ShowError(MessageContainer.LoginPasswordErrorMessage);
            return false;
        }
        return true;
    }

    private User? GetUser(DatabaseContext context) =>
        context.Users
            .AsNoTracking()
            .FirstOrDefault(u => u.Username == Username);

    private bool ValidateUser(User? user)
    {
        if (user == null)
        {
            MessageBoxDisplayer.ShowError(MessageContainer.LoginInvalidUsernameMessage);
            return false;
        }
        if (!PasswordHasher.VerifyPassword(Password, user.Password))
        {
            MessageBoxDisplayer.ShowError(MessageContainer.LoginInvalidPasswordMessage);
            return false;
        }
        return true;
    }

    private void OpenAppropriateWindow(User user)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            if (user.Role == MessageContainer.AdminRole)
            {
                var dashboardView = new DashboardView();
                dashboardView.Show();
            }
            else if (user.Role == MessageContainer.UserRole)
            {
                var dashboardView = new DashboardView();
                dashboardView.Show();
            }
            else
            {
                MessageBoxDisplayer.ShowError(MessageContainer.LoginInvalidUserRoleMessage);
                return;
            }
            CloseCurrentWindow();
        });
    }

    private void CloseCurrentWindow()
    {
        Application.Current.Windows
            .OfType<Window>()
            .SingleOrDefault(w => w.DataContext == this)?.Close();
    }

    private static void HandleException(Exception ex) =>
        MessageBoxDisplayer.ShowError(MessageContainer.GetUnexpectedErrorMessage(ex.Message));
    #endregion
}
