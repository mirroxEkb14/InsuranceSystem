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

public partial class AddHotovostViewModel : ObservableObject
{
    [ObservableProperty] private decimal prijato;
    [ObservableProperty] private decimal vraceno;
    [ObservableProperty] private ObservableCollection<Platba> availablePlatby;
    [ObservableProperty] private Platba selectedPlatba;
    [ObservableProperty] private string? errorMessage;

    private readonly DatabaseContext _context;

    public AddHotovostViewModel(DatabaseContext context)
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
            var newHotovost = CreateHotovost();

            // TODO

            MessageBoxDisplayer.ShowInfo(MessageContainer.AddCashPaymentSuccess);
            Cancel();
        }
        catch (Exception ex)
        {
            ErrorMessage = MessageContainer.GetUnexpectedErrorMessage(ex.Message);
        }
    }

    [RelayCommand]
    public void Cancel() =>
        Application.Current.Windows.OfType<Window>()
            .SingleOrDefault(w => w.DataContext == this)?
            .Close();

    #region Private Helper Methods
    private bool ValidateInputs()
    {
        if (SelectedPlatba == null)
        {
            ErrorMessage = MessageContainer.AddCashPaymentRequiredPayment;
            return false;
        }
        if (Prijato <= 0)
        {
            ErrorMessage = MessageContainer.AddCashPaymentInvalidAmountAccepted;
            return false;
        }
        if (Vraceno < 0)
        {
            ErrorMessage = MessageContainer.AddCashPaymentInvalidAmountReturned;
            return false;
        }
        return true;
    }

    private Hotovost CreateHotovost() =>
        new()
        {
            IdPlatby = SelectedPlatba.IdPlatby,
            Prijato = Prijato,
            Vraceno = Vraceno
        };

    private void LoadPlatby()
    {
        try
        {
            AvailablePlatby = new ObservableCollection<Platba>(_context.Platby.ToList());
        }
        catch (Exception ex)
        {
            ErrorMessage = MessageContainer.GetUnexpectedErrorMessage(ex.Message);
        }
    }
    #endregion
}
