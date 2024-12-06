#region Imports
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InsuranceSystemDemo.Controls;
using InsuranceSystemDemo.Database;
using InsuranceSystemDemo.Models;
using InsuranceSystemDemo.Utils;
using InsuranceSystemDemo.Views;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client; 
using System.Data;
using System.Windows;
#endregion

namespace InsuranceSystemDemo.ViewModels;

public partial class RegisterViewModel : ObservableObject
{
    [ObservableProperty] private string? _username;
    [ObservableProperty] private string? _firstName;
    [ObservableProperty] private string? _lastName;
    [ObservableProperty] private string? _email;
    [ObservableProperty] private string? _phone;
    [ObservableProperty] private string? _password;
    [ObservableProperty] private string? _confirmPassword;
    [ObservableProperty] private string? _errorMessage;

    [RelayCommand]
    public void Register()
    {
        ErrorMessage = null;
        ErrorMessage = RegisterValidator.ValidateUsername(Username)
                       ?? RegisterValidator.ValidateFirstName(FirstName)
                       ?? RegisterValidator.ValidateLastName(LastName)
                       ?? RegisterValidator.ValidateEmail(Email)
                       ?? RegisterValidator.ValidatePhone(Phone)
                       ?? RegisterValidator.ValidatePassword(Password, ConfirmPassword);

        if (!string.IsNullOrEmpty(ErrorMessage))
            return;

        var dbContextOptions = DatabaseContextGetter.GetDatabaseContextOptions();
        try
        {
            using var context = new DatabaseContext(dbContextOptions);
            if (!CheckExistingUser(context))
                return;

            SaveNewUser(context);
            MessageBoxDisplayer.ShowInfo(MessageContainer.RegisterSuccess);
            RedirectToLoginView();
            CloseRegisterWindow();
        }
        catch (Exception ex)
        {
            ErrorMessage = MessageContainer.GetUnexpectedErrorMessage(ex.Message);
        }
    }

    [RelayCommand]
    public void BackToLoginView()
    {
        new LoginView().Show();
        CloseRegisterWindow();
    }

    #region Private Helper Methods
    private bool CheckExistingUser(DatabaseContext context)
    {
        var usernameLower = Username?.ToLower() ?? string.Empty;
        var emailLower = Email?.ToLower() ?? string.Empty;

        var existingUser = context.Users
            .AsNoTracking()
            .FirstOrDefault(u => u.Username != null && u.Username.ToLower() == usernameLower
                              || u.Email != null && u.Email.ToLower() == emailLower
                              || u.Phone == Phone);

        if (existingUser != null)
        {
            if (existingUser.Username?.ToLower() == usernameLower)
                ErrorMessage = MessageContainer.RegisterExistingUsername;
            else if (existingUser.Email?.ToLower() == emailLower)
                ErrorMessage = MessageContainer.RegisterExistingEmail;
            else if (existingUser.Phone == Phone)
                ErrorMessage = MessageContainer.RegisterExistingPhone;

            return false;
        }

        return true;
    }

    private void SaveNewUser(DatabaseContext context)
    {
        string hashedPassword = PasswordHasher.HashPassword(Password!);

        
        var userIdParam = new OracleParameter("p_user_id", OracleDbType.Int32)
        {
            Direction = ParameterDirection.Output
        };

       
        context.Database.ExecuteSqlRaw(
            "BEGIN ADDUSER(:p_username, :p_password, :p_role, :p_first_name, :p_last_name, :p_email, :p_phone, :p_user_id); END;",
            new OracleParameter("p_username", Username),
            new OracleParameter("p_password", hashedPassword),
            new OracleParameter("p_role", MessageContainer.UserRole), // Роль
            new OracleParameter("p_first_name", FirstName),
            new OracleParameter("p_last_name", LastName),
            new OracleParameter("p_email", Email),
            new OracleParameter("p_phone", Phone),
            userIdParam
        );

        
        if (userIdParam.Value != DBNull.Value)
        {
            var oracleDecimal = (Oracle.ManagedDataAccess.Types.OracleDecimal)userIdParam.Value;
            int newUserId = oracleDecimal.ToInt32();
            //MessageBoxDisplayer.ShowInfo($"New user ID: {newUserId}");
        }
        else
        {
            throw new Exception("Failed to create user: ID was not returned.");
        }
    }

    private void RedirectToLoginView()
    {
        var loginView = new LoginView
        {
            DataContext = new LoginViewModel
            {
                Username = Username,
                Password = Password
            }
        };
        loginView.Show();
    }

    private void CloseRegisterWindow()
    {
        Application.Current.Windows
            .OfType<Window>()
            .SingleOrDefault(w => w.DataContext == this)?.Close();
    }
    #endregion
}
