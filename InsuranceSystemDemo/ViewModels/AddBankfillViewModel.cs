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

public partial class AddBankfillViewModel : ObservableObject
{
    [ObservableProperty] private decimal sumaZavazky;
    [ObservableProperty] private DateTime? dataVzniku;
    [ObservableProperty] private DateTime? dataSplatnisti;
    [ObservableProperty] private ObservableCollection<Pohledavka>? availablePohledavky;
    [ObservableProperty] private Pohledavka? selectedPohledavka;
    [ObservableProperty] private string? errorMessage;

    private readonly DatabaseContext _context;

    public AddBankfillViewModel(DatabaseContext context)
    {
        _context = context;
        LoadPohledavky();
    }

    [RelayCommand]
    public void Save()
    {
        ErrorMessage = null;

        if (!ValidateInputs())
            return;

        try
        {
            var newZavazek = CreateBankfill();

            // TODO

            MessageBoxDisplayer.ShowInfo(MessageContainer.AddBankfillSuccess);
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
    private void LoadPohledavky()
    {
        try
        {
            AvailablePohledavky = new ObservableCollection<Pohledavka>(_context.Pohledavky.ToList());
        }
        catch (Exception ex)
        {
            ErrorMessage = MessageContainer.GetUnexpectedErrorMessage(ex.Message);
        }
    }

    private bool ValidateInputs()
    {
        if (SumaZavazky <= 0)
        {
            ErrorMessage = MessageContainer.AddBankfillInvalidAmount;
            return false;
        }
        if (DataVzniku == null)
        {
            ErrorMessage = MessageContainer.AddBankfillRequiredStart;
            return false;
        }
        if (DataSplatnisti == null)
        {
            ErrorMessage = MessageContainer.AddBankfillRequiredEndDate;
            return false;
        }
        if (SelectedPohledavka == null)
        {
            ErrorMessage = MessageContainer.AddBankfillRequiredDebt;
            return false;
        }
        return true;
    }

    private Zavazek CreateBankfill() =>
        new()
        {
            SumaZavazky = SumaZavazky,
            DataVzniku = DataVzniku.Value,
            DataSplatnisti = DataSplatnisti.Value,
            PohledavkaIdPohledavky = SelectedPohledavka?.IdPohledavky ?? 0
        };
    #endregion
}
