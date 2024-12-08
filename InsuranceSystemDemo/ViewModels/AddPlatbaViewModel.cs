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

public partial class AddPlatbaViewModel : ObservableObject
{
    [ObservableProperty] private DateTime? _datumPlatby;
    [ObservableProperty] private decimal _sumaPlatby;
    [ObservableProperty] private string? _typPlatby;
    [ObservableProperty] private ObservableCollection<Klient> _availableClients;
    [ObservableProperty] private ObservableCollection<PojistnaSmlouva> _availablePolicies;
    [ObservableProperty] private Klient? _selectedClient;
    [ObservableProperty] private PojistnaSmlouva? _selectedPolicy;
    [ObservableProperty] private string? _errorMessage;

    private readonly DatabaseContext _context;

    public AddPlatbaViewModel(DatabaseContext context)
    {
        _context = context;
        AvailableClients = LoadAvailableClients();
        AvailablePolicies = LoadAvailableContracts();
    }

    [RelayCommand]
    public void Save()
    {
        ErrorMessage = null;

        if (!ValidateInputs())
            return;

        try
        {
            using var context = new DatabaseContext(DatabaseContextGetter.GetDatabaseContextOptions());
            var newPlatba = CreatePayment();

            // TODO

            MessageBoxDisplayer.ShowInfo(MessageContainer.AddPaymentSuccess);
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
        if (DatumPlatby == null)
        {
            ErrorMessage = MessageContainer.AddPaymentDateRequired;
            return false;
        }
        if (SumaPlatby <= 0)
        {
            ErrorMessage = MessageContainer.AddPaymentInvalidAmount;
            return false;
        }
        if (SelectedClient == null)
        {
            ErrorMessage = MessageContainer.AddPaymentInvalidClient;
            return false;
        }
        if (SelectedPolicy == null)
        {
            ErrorMessage = MessageContainer.AddPaymentInvalidPolicy;
            return false;
        }
        return true;
    }

    private Platba CreatePayment() =>
        new()
        {
            DatumPlatby = DatumPlatby!.Value,
            SumaPlatby = SumaPlatby,
            TypPlatby = TypPlatby,
            KlientIdKlientu = SelectedClient!.IdKlientu,
            PojistnaSmlouvaIdPojistky = SelectedPolicy!.IdPojistky
        };

    private ObservableCollection<Klient> LoadAvailableClients() =>
        new(_context.Klienti.ToList());

    private ObservableCollection<PojistnaSmlouva> LoadAvailableContracts() =>
        new(_context.PojistneSmlouvy.ToList());
    #endregion
}
