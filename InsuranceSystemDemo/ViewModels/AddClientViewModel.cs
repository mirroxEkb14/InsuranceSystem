#region Imports
using System.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using InsuranceSystemDemo.Database;
using InsuranceSystemDemo.Utils;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
#endregion

namespace InsuranceSystemDemo.ViewModels;

public partial class AddClientViewModel : AddItemViewModel
{
    [ObservableProperty] private string? _firstName;
    [ObservableProperty] private string? _lastName;
    [ObservableProperty] private string? _email;
    [ObservableProperty] private string? _phone;
    [ObservableProperty] private DateTime? _birthDate;

    [ObservableProperty] private string? _street;
    [ObservableProperty] private string? _city;
    [ObservableProperty] private string? _country;
    [ObservableProperty] private string? _houseNumber;
    [ObservableProperty] private string? _postalCode;


    public override void Save()
    {
        ErrorMessage = null;

        if (!ValidateInputs())
            return;

        using var context = new DatabaseContext(DatabaseContextGetter.GetDatabaseContextOptions());
        try
        {
            var addressId = CreateAddress(context);
            HandleClient(context, addressId);
            //MessageBoxDisplayer.ShowInfo(MessageContainer.AddClientSuccess);
            Cancel();
        }
        catch (Exception ex)
        {
            ErrorMessage = MessageContainer.GetUnexpectedErrorMessage(ex.Message);
        }
    }

    #region Private Helper Methods
    private bool ValidateInputs()
    {
        if (string.IsNullOrWhiteSpace(FirstName))
        {
            ErrorMessage = MessageContainer.AddClientRequiredFirstName;
            return false;
        }
        if (string.IsNullOrWhiteSpace(LastName))
        {
            ErrorMessage = MessageContainer.AddClientRequiredLastName;
            return false;
        }
        if (string.IsNullOrWhiteSpace(Email))
        {
            ErrorMessage = MessageContainer.AddClientRequiredEmail;
            return false;
        }
        if (!Email.Contains("@"))
        {
            ErrorMessage = MessageContainer.AddClientInvalidEmail;
            return false;
        }
        if (string.IsNullOrWhiteSpace(Phone))
        {
            ErrorMessage = MessageContainer.AddClientRequiredPhone;
            return false;
        }
        if (string.IsNullOrWhiteSpace(Street))
        {
            ErrorMessage = MessageContainer.AddClientRequiredStreet;
            return false;
        }
        if (string.IsNullOrWhiteSpace(City))
        {
            ErrorMessage = MessageContainer.AddClientRequiredCity;
            return false;
        }
        if (string.IsNullOrWhiteSpace(Country))
        {
            ErrorMessage = MessageContainer.AddClientRequiredCountry;
            return false;
        }
        if (string.IsNullOrWhiteSpace(HouseNumber))
        {
            ErrorMessage = MessageContainer.AddClientRequiredHouseNumber;
            return false;
        }
        if (string.IsNullOrWhiteSpace(PostalCode))
        {
            ErrorMessage = MessageContainer.AddClientRequiredPostalCode;
            return false;
        }
        return true;
    }

    private int CreateAddress(DatabaseContext context)
    {
        var addressId = new OracleParameter("p_id_adresa", OracleDbType.Int32)
        {
            Direction = ParameterDirection.Output
        };

        context.Database.ExecuteSqlRaw(
            "BEGIN ADDADDRESS(:p_ulice, :p_mesto, :p_stat, :p_cislo_popisne, :p_psc, :p_id_adresa); END;",
            new OracleParameter("p_ulice", Street),
            new OracleParameter("p_mesto", City),
            new OracleParameter("p_stat", Country),
            new OracleParameter("p_cislo_popisne", Convert.ToInt32(HouseNumber)),
            new OracleParameter("p_psc", Convert.ToInt32(PostalCode)),
            addressId
        );

        var oracleDecimal = (OracleDecimal)addressId.Value;
        return oracleDecimal.ToInt32();
    }

    private void HandleClient(DatabaseContext context, int addressId)
    {
        var clientId = new OracleParameter("p_id_klient", OracleDbType.Decimal)
        {
            Direction = ParameterDirection.Output
        };

        context.Database.ExecuteSqlRaw(
            "BEGIN ADDCLIENT(:p_jmeno, :p_prijmeni, :p_email, :p_telefon, :p_datum_narozeni, :p_adresa_id, :p_id_klient); END;",
            new OracleParameter("p_jmeno", FirstName),
            new OracleParameter("p_prijmeni", LastName),
            new OracleParameter("p_email", Email),
            new OracleParameter("p_telefon", Convert.ToInt64(Phone)),
            new OracleParameter("p_datum_narozeni", BirthDate.HasValue ? BirthDate.Value : DBNull.Value),
            new OracleParameter("p_adresa_id", addressId),
            clientId
        );

        if (clientId.Value != DBNull.Value)
        {
            var oracleDecimal = (OracleDecimal)clientId.Value;
            int newClientId = oracleDecimal.ToInt32();
            //MessageBox.Show($"New client ID: {newClientId}");
        }
        else
        {
            throw new Exception("Client ID is null.");
        }
    }



    #endregion
}
