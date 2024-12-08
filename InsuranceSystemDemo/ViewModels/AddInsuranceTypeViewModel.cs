#region Imports
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InsuranceSystemDemo.Database;
using InsuranceSystemDemo.Utils;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System.Windows;
using System.Text;
#endregion

namespace InsuranceSystemDemo.ViewModels;

public partial class AddInsuranceTypeViewModel(DatabaseContext context) : ObservableObject
{
    [ObservableProperty] private string? _dostupnost;
    [ObservableProperty] private string? _podminky;
    [ObservableProperty] private string? _popis;
    [ObservableProperty] private decimal _maximalneKryti;
    [ObservableProperty] private decimal _minimalneKryti;
    [ObservableProperty] private DateTime _datimZacatku = DateTime.Now;
    [ObservableProperty] private string? _errorMessage;

    private readonly DatabaseContext _context = context;

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
            MessageBoxDisplayer.ShowInfo(MessageContainer.AddInsuranceTypeSuccess);
            CloseWindow();
        }
        catch (Exception ex)
        {
            HandleSaveException(ex);
        }
    }

    [RelayCommand]
    private void Cancel() => CloseWindow();

    #region Private Helper Methods
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
            ErrorMessage = MessageContainer.AddInsuranceTypeRequiredAvailability;
            return false;
        }
        if (string.IsNullOrWhiteSpace(Podminky))
        {
            ErrorMessage = MessageContainer.AddInsuranceTypeRequiredConditions;
            return false;
        }
        if (MaximalneKryti <= 0)
        {
            ErrorMessage = MessageContainer.AddInsuranceTypeInvalidMaxCoverage;
            return false;
        }
        if (MinimalneKryti <= 0)
        {
            ErrorMessage = MessageContainer.AddInsuranceTypeInvalidMinCoverage;
            return false;
        }
        if (MinimalneKryti > MaximalneKryti)
        {
            ErrorMessage = MessageContainer.AddInsuranceTypeInvalidCoverage;
            return false;
        }
        return true;
    }

    private void HandleSaveException(Exception ex)
    {
        var errorDetails = new StringBuilder();
        errorDetails.AppendLine("An error occurred:");

        var currentException = ex;
        while (currentException != null)
        {
            errorDetails.AppendLine($"Message: {currentException.Message}");
            errorDetails.AppendLine($"StackTrace: {currentException.StackTrace}");
            currentException = currentException.InnerException;
        }
        MessageBoxDisplayer.ShowError(errorDetails.ToString());
        ErrorMessage = errorDetails.ToString();
    }
    #endregion
}
