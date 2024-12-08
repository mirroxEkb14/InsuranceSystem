#region Imports
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InsuranceSystemDemo.Database;
using InsuranceSystemDemo.Models;
using InsuranceSystemDemo.Utils;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using Microsoft.EntityFrameworkCore;
#endregion

namespace InsuranceSystemDemo.ViewModels;

public partial class AddPohledavkaViewModel : ObservableObject
{
    [ObservableProperty] private decimal _sumaPohledavky;
    [ObservableProperty] private DateTime _datumZacatku;
    [ObservableProperty] private DateTime _datumKonce;
    [ObservableProperty] private ObservableCollection<PojistnaSmlouva> _availablePojistneSmlouvy;
    [ObservableProperty] private PojistnaSmlouva? _selectedPojistnaSmlouva;
    [ObservableProperty] private string? _errorMessage;

    private readonly Action _onClose;

    public AddPohledavkaViewModel(Action onClose)
    {
        _onClose = onClose;

        _datumZacatku = DateTime.Now;
        _datumKonce = DateTime.Now.AddYears(1);

        using var context = new DatabaseContext(DatabaseContextGetter.GetDatabaseContextOptions());
        AvailablePojistneSmlouvy = new ObservableCollection<PojistnaSmlouva>(context.PojistneSmlouvy.ToList());
    }

    [RelayCommand]
    public void Save()
    {
        try
        {
            if (!ValidateInputs())
                return;

            var context = new DatabaseContext(DatabaseContextGetter.GetDatabaseContextOptions());
            AddDebtToDatabase(context);

            MessageBoxDisplayer.ShowInfo($"{MessageContainer.AddDebtSuccess}");
            CloseWindow();
        }
        catch (Exception ex)
        {
            MessageBoxDisplayer.ShowError(ex.Message);
        }
    }

    [RelayCommand]
    public void Cancel() =>
        CloseWindow();

    #region Private Helper Methods
    private void CloseWindow()
    {
        Application.Current.Windows
            .OfType<Window>()
            .SingleOrDefault(w => w.DataContext == this)?.Close();
    }

    private bool ValidateInputs()
    {
        if (SumaPohledavky <= 0)
        {
            ErrorMessage = MessageContainer.AddDebtInvalidAmount;
            return false;
        }
        if (DatumKonce <= DatumZacatku)
        {
            ErrorMessage = MessageContainer.AddDebtInvalidDates;
            return false;
        }
        if (SelectedPojistnaSmlouva == null)
        {
            ErrorMessage = MessageContainer.AddDebtInvalidContract;
            return false;
        }
        return true;
    }
    #endregion


    private int AddDebtToDatabase(DatabaseContext context)
    {
        var idParam = new OracleParameter("p_id_pohledavky", OracleDbType.Decimal)
        {
            Direction = ParameterDirection.Output
        };

        context.Database.ExecuteSqlRaw(
            "BEGIN ADD_POHLEDAVKA(:p_suma_pohledavky, :p_datum_zacatku, :p_datum_konce, :p_pojistnasmlouva_id_pojistky, :p_id_pohledavky); END;",
            new OracleParameter("p_suma_pohledavky", SumaPohledavky),
            new OracleParameter("p_datum_zacatku", DatumZacatku),
            new OracleParameter("p_datum_konce", DatumKonce),
            new OracleParameter("p_pojistnasmlouva_id_pojistky", SelectedPojistnaSmlouva?.IdPojistky),
            idParam
        );

        if (idParam.Value is OracleDecimal oracleDecimal)
            return oracleDecimal.ToInt32();

        throw new Exception(MessageContainer.AddDebtErrorRetrievingID);
    }
    
}
