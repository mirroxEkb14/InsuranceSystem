#region Imports
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InsuranceSystemDemo.Controls;
using InsuranceSystemDemo.Database;
using InsuranceSystemDemo.Models;
using InsuranceSystemDemo.Utils;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System.Windows;
#endregion

namespace InsuranceSystemDemo.ViewModels;

public partial class AddAddressViewModel : ObservableObject
{
    [ObservableProperty] private string? _ulice;
    [ObservableProperty] private string? _mesto;
    [ObservableProperty] private string? _stat;
    [ObservableProperty] private string? _cisloPopisne;
    [ObservableProperty] private string? _psc;
    [ObservableProperty] private string? _errorMessage;

    [RelayCommand]
    public void Save()
    {
        ErrorMessage = null;

        if (!ValidateInputs())
            return;

        try
        {
            using var context = new DatabaseContext(DatabaseContextGetter.GetDatabaseContextOptions());

            
            var newAddressId = AddAddressUsingProcedure(context);

            MessageBoxDisplayer.ShowInfo($"Address successfully added");
            Cancel();
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
    private bool ValidateInputs()
    {
        if (string.IsNullOrWhiteSpace(Ulice))
        {
            ErrorMessage = MessageContainer.AddAddressRequiredStreet;
            return false;
        }
        if (string.IsNullOrWhiteSpace(Mesto))
        {
            ErrorMessage = MessageContainer.AddAddressRequiredCity;
            return false;
        }
        if (string.IsNullOrWhiteSpace(Stat))
        {
            ErrorMessage = MessageContainer.AddAddressRequiredCountry;
            return false;
        }
        if (string.IsNullOrWhiteSpace(CisloPopisne))
        {
            ErrorMessage = MessageContainer.AddAddressRequiredHouseNumber;
            return false;
        }
        if (!int.TryParse(CisloPopisne, out _))
        {
            ErrorMessage = MessageContainer.AddAddressInvalidHouseNumber;
            return false;
        }
        if (string.IsNullOrWhiteSpace(Psc))
        {
            ErrorMessage = MessageContainer.AddAddressInvalidPostalCode;
            return false;
        }
        if (!int.TryParse(Psc, out _))
        {
            ErrorMessage = MessageContainer.AddAddressInvalidPostalCode;
            return false;
        }
        return true;
    }

    private int AddAddressUsingProcedure(DatabaseContext context)
    {
        // Параметр для возврата ID нового адреса
        var addressIdParam = new OracleParameter("p_id_adresa", OracleDbType.Int32)
        {
            Direction = System.Data.ParameterDirection.Output
        };

        // Выполнение процедуры
        context.Database.ExecuteSqlRaw(
            "BEGIN ADD_ADDRESS(:p_ulice, :p_mesto, :p_stat, :p_cislo_popisne, :p_psc, :p_id_adresa); END;",
            new OracleParameter("p_ulice", Ulice),
            new OracleParameter("p_mesto", Mesto),
            new OracleParameter("p_stat", Stat),
            new OracleParameter("p_cislo_popisne", Convert.ToInt32(CisloPopisne)),
            new OracleParameter("p_psc", Convert.ToInt32(Psc)),
            addressIdParam
        );

        // Преобразование возвращённого значения в int
        if (addressIdParam.Value is Oracle.ManagedDataAccess.Types.OracleDecimal oracleDecimal)
        {
            return oracleDecimal.ToInt32();
        }

        throw new Exception("Failed to retrieve the new address ID.");
    }
    #endregion
}
