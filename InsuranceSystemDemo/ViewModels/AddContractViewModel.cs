#region Imports
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InsuranceSystemDemo.Database;
using InsuranceSystemDemo.Models;
using InsuranceSystemDemo.Utils;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Text;
#endregion

namespace InsuranceSystemDemo.ViewModels;

public partial class AddContractViewModel : ObservableObject
{
    [ObservableProperty] private decimal _pojistnaCastka;
    [ObservableProperty] private DateTime _datumZacatkuPlatnosti;
    [ObservableProperty] private DateTime _datumUkonceniPlatnosti;
    [ObservableProperty] private DateTime _dataVystaveni;
    [ObservableProperty] private decimal _cena;
    [ObservableProperty] private ObservableCollection<Klient>? _availableClients;
    [ObservableProperty] private ObservableCollection<Pobocka>? _availableBranches;
    [ObservableProperty] private ObservableCollection<TypPojistky>? _availablePolicyTypes;
    [ObservableProperty] private Klient? _selectedClient;
    [ObservableProperty] private Pobocka? _selectedBranch;
    [ObservableProperty] private TypPojistky? _selectedPolicyType;
    [ObservableProperty] private string? _errorMessage;

    private readonly DatabaseContext _context;

    public AddContractViewModel(DatabaseContext context)
    {
        _context = context;
       
        DatumZacatkuPlatnosti = DateTime.Now;
        DatumUkonceniPlatnosti = DateTime.Now.AddYears(1); 
        DataVystaveni = DateTime.Now;

        LoadData();
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
                "BEGIN AddPojistnaMlouva(:p_POJISTNA_CASTKA, :p_DATUM_ZACATKU_PLATNOSTI, :p_DATUM_UKONCENI_PLATNOSTI, :p_DATA_VYSTAVENI, :p_CENA, :p_KLIENT_ID_KLIENTU, :p_POBOCKY_ID_POBOCKY, :p_TYPPOJISTKY_ID_TYP); END;",
                new OracleParameter("p_POJISTNA_CASTKA", PojistnaCastka),
                new OracleParameter("p_DATUM_ZACATKU_PLATNOSTI", DatumZacatkuPlatnosti),
                new OracleParameter("p_DATUM_UKONCENI_PLATNOSTI", DatumUkonceniPlatnosti),
                new OracleParameter("p_DATA_VYSTAVENI", DataVystaveni),
                new OracleParameter("p_CENA", Cena),
                new OracleParameter("p_KLIENT_ID_KLIENTU", SelectedClient?.IdKlientu ?? (object)DBNull.Value),
                new OracleParameter("p_POBOCKY_ID_POBOCKY", SelectedBranch?.IdPobocky ?? (object)DBNull.Value),
                new OracleParameter("p_TYPPOJISTKY_ID_TYP", SelectedPolicyType?.IdTyp ?? (object)DBNull.Value)
            );

            MessageBoxDisplayer.ShowInfo(MessageContainer.AddContractSuccess);
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

    private void LoadData()
    {
        AvailableClients = new ObservableCollection<Klient>(
            _context.Klienti
            .OrderBy(k => k.Jmeno) 
            .ThenBy(k => k.Prijmeni) 
            .ToList()
        );
        AvailableBranches = new ObservableCollection<Pobocka>(
            _context.Pobocky
            .OrderBy(p => p.Nazev) 
            .ToList()
        );
        AvailablePolicyTypes = new ObservableCollection<TypPojistky>(
            _context.TypPojistky
            .OrderBy(t => t.Popis) 
            .ToList()
        );

    }
    private bool ValidateInputs()
    {
        if (PojistnaCastka <= 0)
        {
            ErrorMessage = MessageContainer.AddContractInvalidAmount;
            return false;
        }
        if (DatumUkonceniPlatnosti <= DatumZacatkuPlatnosti)
        {
            ErrorMessage = MessageContainer.AddContractInvalidDates;
            return false;
        }
        if (SelectedClient == null)
        {
            ErrorMessage = MessageContainer.AddContractInvalidClient;
            return false;
        }
        if (SelectedBranch == null)
        {
            ErrorMessage = MessageContainer.AddContractInvalidBranch;
            return false;
        }
        if (SelectedPolicyType == null)
        {
            ErrorMessage = MessageContainer.AddContractInvalidPolicyType;
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

        MessageBox.Show(errorDetails.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        ErrorMessage = errorDetails.ToString();
    }
    #endregion
}
