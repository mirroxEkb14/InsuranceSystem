using CommunityToolkit.Mvvm.ComponentModel;
using InsuranceSystemDemo.Controls;
using InsuranceSystemDemo.Database;
using InsuranceSystemDemo.Models;
using InsuranceSystemDemo.Utils;

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
            var newAddress = CreateAddress(context);
            HandleClient(context, newAddress.IdAdresa);
            MessageBoxDisplayer.ShowInfo(MessageContainer.AddClientSuccess);
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

    private Adresa CreateAddress(DatabaseContext context)
    {
        var newAddress = new Adresa
        {
            Ulice = Street,
            Mesto = City,
            Stat = Country,
            CisloPopisne = HouseNumber,
            PSC = PostalCode
        };
        context.Adresy.Add(newAddress);
        context.SaveChanges();
        return newAddress;
    }

    private void HandleClient(DatabaseContext context, int addressId)
    {
        var newClient = new Klient
        {
            Jmeno = FirstName,
            Prijmeni = LastName,
            Email = Email,
            Telefon = Phone,
            AdresaId = addressId,
            DatumNarozeni = BirthDate
        };
        context.Klienti.Add(newClient);
        context.SaveChanges();
    }
    #endregion
}
