#region Imports
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InsuranceSystemDemo.Database;
using InsuranceSystemDemo.Models;
using InsuranceSystemDemo.Utils;
using System.Collections.ObjectModel;
using System.Windows;
#endregion

namespace InsuranceSystemDemo.ViewModels;

public partial class AddKartaViewModel : ObservableObject
{
    [ObservableProperty] private int _cisloKarty;
    [ObservableProperty] private int _cisloUctu;
    [ObservableProperty] private Platba? _selectedPlatba;
    [ObservableProperty] private ObservableCollection<Platba>? _availablePlatby;
    [ObservableProperty] private string? _errorMessage;

    private readonly DatabaseContext _context;

    public AddKartaViewModel(DatabaseContext context)
    {
        _context = context;
        LoadPlatby();
    }

    [RelayCommand]
    public void Save()
    {
        ErrorMessage = null;

        if (!ValidateInputs())
            return;

        try
        {
            var karta = CreateCard();

            // TODO

            MessageBoxDisplayer.ShowInfo(MessageContainer.AddKartaSuccess);
            CloseWindow();
        }
        catch (Exception ex)
        {
            ErrorMessage = MessageContainer.GetUnexpectedErrorMessage(ex.Message);
        }
    }

    [RelayCommand]
    public void Cancel() =>
        CloseWindow();

    #region Private Helper Methods
    private bool ValidateInputs()
    {
        if (CisloKarty <= 0)
        {
            ErrorMessage = MessageContainer.AddKartaInvalidCardNumber;
            return false;
        }
        if (CisloUctu <= 0)
        {
            ErrorMessage = MessageContainer.AddKartaInvalidAccountNumber;
            return false;
        }
        if (SelectedPlatba == null)
        {
            ErrorMessage = MessageContainer.AddKartaInvalidRequiredPayment;
            return false;
        }
        return true;
    }

    private Karta CreateCard() =>
        new()
        {
            CisloKarty = CisloKarty,
            CisloUctu = CisloUctu,
            IdPlatby = SelectedPlatba!.IdPlatby
        };

    private void CloseWindow()
    {
        Application.Current.Windows
            .OfType<Window>()
            .SingleOrDefault(w => w.DataContext == this)
            ?.Close();
    }

    private void LoadPlatby()
    {
        var platby = _context.Platby.ToList();
        AvailablePlatby = new ObservableCollection<Platba>(platby);
    }
    #endregion
}
