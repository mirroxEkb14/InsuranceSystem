#region Imports
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InsuranceSystemDemo.Database;
using InsuranceSystemDemo.Models;
using InsuranceSystemDemo.Utils;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
#endregion

namespace InsuranceSystemDemo.ViewModels;

public partial class AddBankfillViewModel : ObservableObject
{
    [ObservableProperty] private decimal sumaZavazky;
    [ObservableProperty] private DateTime? dataVzniku;
    [ObservableProperty] private DateTime? dataSplatnisti;
    [ObservableProperty] private ObservableCollection<Pohledavka>? availablePohledavky;
    [ObservableProperty] private Pohledavka? selectedPohledavka;
    [ObservableProperty] private string? errorMessage;

    private readonly DatabaseContext _context;

    public AddBankfillViewModel(DatabaseContext context)
    {
        _context = context;
        LoadPohledavky();
    }

    [RelayCommand]
    private void Save()
    {
        ErrorMessage = null;

        if (!ValidateInputs())
            return;

        try
        {
            var idParam = new OracleParameter("p_id_zavazky", OracleDbType.Decimal)
            {
                Direction = ParameterDirection.Output
            };

            _context.Database.ExecuteSqlRaw(
                "BEGIN ADD_ZAVAZKY(:p_id_zavazky, :p_suma_zavazky, :p_data_vzniku, :p_data_splatnosti, :p_pohledavka_id_pohledavky); END;",
                idParam,
                new OracleParameter("p_suma_zavazky", SumaZavazky),
                new OracleParameter("p_data_vzniku", DataVzniku),
                new OracleParameter("p_data_splatnosti", DataSplatnisti),
                new OracleParameter("p_pohledavka_id_pohledavky", SelectedPohledavka?.IdPohledavky)
            );

            if (idParam.Value is OracleDecimal oracleDecimal)
            {
                var newId = oracleDecimal.ToInt32();
                MessageBoxDisplayer.ShowInfo($"{MessageContainer.AddBankfillSuccess} ID: {newId}");
            }

            CloseWindow();
        }
        catch (Exception ex)
        {
            ErrorMessage = MessageContainer.GetUnexpectedErrorMessage(ex.Message);
        }
    }

    [RelayCommand]
    public void Cancel()
    {
        Application.Current.Windows
            .OfType<Window>()
            .SingleOrDefault(w => w.DataContext == this)?.Close();
    }

    #region Private Helper Methods
    private void LoadPohledavky()
    {
        try
        {
            AvailablePohledavky = new ObservableCollection<Pohledavka>(_context.Pohledavky.ToList());
        }
        catch (Exception ex)
        {
            ErrorMessage = MessageContainer.GetUnexpectedErrorMessage(ex.Message);
        }
    }

    private bool ValidateInputs()
    {
        if (SumaZavazky <= 0)
        {
            ErrorMessage = MessageContainer.AddBankfillInvalidAmount;
            return false;
        }
        if (DataVzniku == null)
        {
            ErrorMessage = MessageContainer.AddBankfillRequiredStart;
            return false;
        }
        if (DataSplatnisti == null)
        {
            ErrorMessage = MessageContainer.AddBankfillRequiredEndDate;
            return false;
        }
        if (SelectedPohledavka == null)
        {
            ErrorMessage = MessageContainer.AddBankfillRequiredDebt;
            return false;
        }
        return true;
    }

    private void CloseWindow()
    {
        Application.Current.Windows
            .OfType<Window>()
            .SingleOrDefault(w => w.DataContext == this)?.Close();
    }
    #endregion
}
