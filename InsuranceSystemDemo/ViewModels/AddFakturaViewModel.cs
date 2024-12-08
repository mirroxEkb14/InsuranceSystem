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

public partial class AddFakturaViewModel : ObservableObject
{
    [ObservableProperty] private decimal _cisloUctu;
    [ObservableProperty] private DateTime? _datumSplatnosti;
    [ObservableProperty] private Platba? _selectedPlatba;
    [ObservableProperty] private ObservableCollection<Platba>? _availablePlatby;
    [ObservableProperty] private string? _errorMessage;

    private readonly DatabaseContext _context;

    public AddFakturaViewModel(DatabaseContext context)
    {
        _context = context;
        LoadPayments();
    }

    [RelayCommand]
    public void Save()
    {
        if (!ValidateInputs())
            return;

        try
        {
            var newFaktura = CreateFaktura();

            // TODO

            MessageBoxDisplayer.ShowInfo(MessageContainer.AddFakturaSuccess);
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
        if (CisloUctu == 0)
        {
            ErrorMessage = MessageContainer.AddFakturaInvalidAccountNumber;
            return false;
        }
        if (SelectedPlatba == null)
        {
            ErrorMessage = MessageContainer.AddFakturaInvalidPayment;
            return false;
        }
        if (DatumSplatnosti == null)
        {
            ErrorMessage = MessageContainer.AddFakturaInvalidDate;
            return false;
        }
        return true;
    }

    private Faktura CreateFaktura() =>
        new()
        {
            IdPlatby = SelectedPlatba!.IdPlatby,
            CisloUctu = CisloUctu,
            DatumSplatnosti = DatumSplatnosti!.Value
        };

    private void CloseWindow() =>
        Application.Current.Windows.OfType<Window>()
            .SingleOrDefault(w => w.IsActive)?
            .Close();

    private void LoadPayments() =>
        AvailablePlatby = new ObservableCollection<Platba>(_context.Platby.ToList());
    #endregion
}
