#region Imports
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InsuranceSystemDemo.Controls;
using InsuranceSystemDemo.Database;
using InsuranceSystemDemo.Models;
using InsuranceSystemDemo.Utils;
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
            var newAddress = CreateAddress();

            using var context = new DatabaseContext(DatabaseContextGetter.GetDatabaseContextOptions());
            context.Adresy.Add(newAddress);
            context.SaveChanges();

            MessageBoxDisplayer.ShowInfo(MessageContainer.AddAddressSuccess);
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

    #region Provate Helper Methods
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

    private Adresa CreateAddress() =>
        new()
        {
            Ulice = Ulice,
            Mesto = Mesto,
            Stat = Stat,
            CisloPopisne = CisloPopisne,
            PSC = Psc
        };
    #endregion
}
