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

public partial class AddPohledavkaViewModel : ObservableObject
{
    [ObservableProperty] private decimal _sumaPohledavky;
    [ObservableProperty] private DateTime _datumZacatku;
    [ObservableProperty] private DateTime _datumKonce;
    [ObservableProperty] private ObservableCollection<PojistnaSmlouva>? _availablePojistneSmlouvy;
    [ObservableProperty] private PojistnaSmlouva? _selectedPojistnaSmlouva;
    [ObservableProperty] private string? _errorMessage;

    private readonly DatabaseContext _context;

    public AddPohledavkaViewModel(DatabaseContext context)
    {
        _datumZacatku = DateTime.Now;
        _datumKonce = DateTime.Now.AddYears(1);

        _context = context;
        LoadAvailablePojistneSmlouvy();
    }

    [RelayCommand]
    public void Save()
    {
        ErrorMessage = null;

        if (!ValidateInputs())
            return;

        try
        {
            var newPohledavka = CreateDebt();

            // TODO: Adding logic using a DB procedure.

            MessageBoxDisplayer.ShowInfo(MessageContainer.AddDebtSuccess);
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
    private void LoadAvailablePojistneSmlouvy() =>
        AvailablePojistneSmlouvy = new ObservableCollection<PojistnaSmlouva>(_context.PojistnaSmlouva.ToList());

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

    private Pohledavka CreateDebt() =>
        new()
        {
            SumaPohledavky = SumaPohledavky,
            DatumZacatku = DatumZacatku,
            DatumKonce = DatumKonce,
            PojistnaSmlouvaId = SelectedPojistnaSmlouva?.IdPojistky ?? 0
        };
    #endregion
}
