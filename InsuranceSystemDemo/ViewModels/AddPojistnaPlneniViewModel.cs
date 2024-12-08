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

public partial class AddPojistnaPlneniViewModel : ObservableObject
{
    [ObservableProperty] private decimal _sumaPlneni;
    [ObservableProperty] private ObservableCollection<PojistnaSmlouva> _availablePojistneSmlouvy;
    [ObservableProperty] private PojistnaSmlouva? _selectedPojistnaSmlouva;
    [ObservableProperty] private ObservableCollection<Zavazek> _availableZavazky;
    [ObservableProperty] private Zavazek? _selectedZavazek;
    [ObservableProperty] private string? _errorMessage;

    private readonly DatabaseContext _context;

    public AddPojistnaPlneniViewModel(DatabaseContext context)
    {
        _context = context;

        AvailablePojistneSmlouvy = new ObservableCollection<PojistnaSmlouva>(_context.PojistneSmlouvy.ToList());
        AvailableZavazky = new ObservableCollection<Zavazek>(_context.Zavazky.ToList());
    }

    [RelayCommand]
    private void Save()
    {
        ErrorMessage = null;

        if (!ValidateInputs())
            return;

        try
        {
            var newPojistnaPlneni = CreateInsuranceFulfilment();

            // TODO

            MessageBoxDisplayer.ShowInfo(MessageContainer.AddInsuranceFulfilmentSuccess);
            Cancel();
        }
        catch (Exception ex)
        {
            ErrorMessage = MessageContainer.GetUnexpectedErrorMessage(ex.Message);
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        Application.Current.Windows
            .OfType<Window>()
            .SingleOrDefault(w => w.DataContext == this)?.Close();
    }

    #region Private Helper Methods
    private bool ValidateInputs()
    {
        if (SumaPlneni <= 0)
        {
            ErrorMessage = MessageContainer.AddInsuranceFulfilmentInvalidAmount;
            return false;
        }
        if (SelectedPojistnaSmlouva == null)
        {
            ErrorMessage = MessageContainer.AddInsuranceFulfilmentRequiredContract;
            return false;
        }
        if (SelectedZavazek == null)
        {
            ErrorMessage = MessageContainer.AddInsuranceFulfilmentRequiredDebt;
            return false;
        }
        return true;
    }

    private PojistnaPlneni CreateInsuranceFulfilment() =>
        new()
        {
            SumaPlneni = SumaPlneni,
            PojistnaSmlouvaIdPojistky = SelectedPojistnaSmlouva?.IdPojistky ?? 0,
            ZavazkyIdZavazky = SelectedZavazek?.IdZavazky ?? 0
        };
    #endregion
}
