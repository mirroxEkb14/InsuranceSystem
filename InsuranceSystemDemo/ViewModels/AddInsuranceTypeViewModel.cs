using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InsuranceSystemDemo.Database;
using InsuranceSystemDemo.Models;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Windows;

namespace InsuranceSystemDemo.ViewModels;

public partial class AddInsuranceTypeViewModel : ObservableObject
{
    private readonly DatabaseContext _context;

    [ObservableProperty]
    private string? _dostupnost;

    [ObservableProperty]
    private string? _podminky;

    [ObservableProperty]
    private string? _popis;

    [ObservableProperty]
    private decimal _maximalneKryti;

    [ObservableProperty]
    private decimal _minimalneKryti;

    [ObservableProperty]
    private DateTime _datimZacatku = DateTime.Now;

    [ObservableProperty]
    private string? _errorMessage;

    public AddInsuranceTypeViewModel(DatabaseContext context)
    {
        _context = context;
    }

    [RelayCommand]
    private void Save()
    {
        ErrorMessage = null;

        if (!ValidateInputs())
            return;

        try
        {
            
            _context.Database.ExecuteSqlRaw(
                "BEGIN AddTypPojistky(:p_DOSTUPNOST, :p_PODMINKY, :p_POPIS, :p_MAXIMALNE_KRYTI, :p_MINIMALNE_KRYTI, :p_DATIM_ZACATKU); END;",
                new OracleParameter("p_DOSTUPNOST", Dostupnost),
                new OracleParameter("p_PODMINKY", Podminky ?? (object)DBNull.Value),
                new OracleParameter("p_POPIS", Popis ?? (object)DBNull.Value),
                new OracleParameter("p_MAXIMALNE_KRYTI", MaximalneKryti),
                new OracleParameter("p_MINIMALNE_KRYTI", MinimalneKryti),
                new OracleParameter("p_DATIM_ZACATKU", DatimZacatku)
            );

           
            MessageBox.Show("Insurance type added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            CloseWindow();
        }
        catch (Exception ex)
        {
            
            var errorDetails = new System.Text.StringBuilder();
            errorDetails.AppendLine("An error occurred:");

            var currentException = ex;
            while (currentException != null)
            {
                errorDetails.AppendLine($"Message: {currentException.Message}");
                errorDetails.AppendLine($"StackTrace: {currentException.StackTrace}");
                currentException = currentException.InnerException;
            }

          
            MessageBox.Show(errorDetails.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);

           
            ErrorMessage = errorDetails.ToString();
        }
    }

    [RelayCommand]
    private void Cancel() => CloseWindow();

    private void CloseWindow()
    {
        foreach (Window window in Application.Current.Windows)
        {
            if (window.DataContext == this)
            {
                window.Close();
                break;
            }
        }
    }

    private bool ValidateInputs()
    {
        if (string.IsNullOrWhiteSpace(Dostupnost))
        {
            ErrorMessage = "Availability is required.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(Podminky))
        {
            ErrorMessage = "Conditions are required.";
            return false;
        }

        if (MaximalneKryti <= 0)
        {
            ErrorMessage = "Maximum coverage must be greater than zero.";
            return false;
        }

        if (MinimalneKryti <= 0)
        {
            ErrorMessage = "Minimum coverage must be greater than zero.";
            return false;
        }

        if (MinimalneKryti > MaximalneKryti)
        {
            ErrorMessage = "Minimum coverage cannot exceed maximum coverage.";
            return false;
        }

        return true;
    }
}
