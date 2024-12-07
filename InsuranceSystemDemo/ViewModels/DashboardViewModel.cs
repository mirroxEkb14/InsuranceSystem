#region Imports
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InsuranceSystemDemo.Database;
using InsuranceSystemDemo.Models;
using InsuranceSystemDemo.Utils;
using InsuranceSystemDemo.Views;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows;
#endregion

namespace InsuranceSystemDemo.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<object>? _currentTableData;
    [ObservableProperty] private string? _searchQuery;
    [ObservableProperty] private string _currentTableName;
    [ObservableProperty] private object? selectedItem;

    private readonly DatabaseContext _context;

    public DashboardViewModel(DatabaseContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        CurrentTableName = MessageContainer.KlientiTableName;
        SwitchToKlientiCommand.Execute(null);
    }

    //
    // Summary:
    //     Is marked as «public», because is used in the «Dashboard.xaml.cs» code-behind to handle
    //         the «DataGrid» cell editing.\

   


    public void SwitchToCurrentTable()
    {
        switch (CurrentTableName)
        {
            case MessageContainer.KlientiTableName:
                SwitchToKlienti();
                break;
            case MessageContainer.AdresyTableName:
                SwitchToAdresy();
                break;
            case MessageContainer.PobockyTableName:
                SwitchToPobocky();
                break;
            case MessageContainer.TypPojistkyTableName:
                SwitchToTypPojistky();
                break;
            case MessageContainer.ContractsTableName:
                SwitchToPojistnaSmlouva(); 
                break;

            default:
                CurrentTableData = new ObservableCollection<object>();
                break;
        }
    }



    #region Switch Tables Commands
    [RelayCommand]
    public void SwitchToKlienti()
    {
        CurrentTableName = MessageContainer.KlientiTableName;
        var clients = _context.Klienti
            .Include(k => k.Adresa)
            .ToList();
        CurrentTableData = new ObservableCollection<object>(clients);
    }

    [RelayCommand]
    public void SwitchToAdresy()
    {
        CurrentTableName = MessageContainer.AdresyTableName;
        var addresses = _context.Adresy.ToList();
        CurrentTableData = new ObservableCollection<object>(addresses);
    }

    [RelayCommand]
    public void SwitchToPobocky()
    {
        CurrentTableName = MessageContainer.PobockyTableName;
        var pobocky = _context.Pobocky
            .Include(p => p.Adresa)
            .ToList();
        CurrentTableData = new ObservableCollection<object>(pobocky);
    }

    [RelayCommand]
    public void SwitchToTypPojistky()
    {
        CurrentTableName = MessageContainer.TypPojistkyTableName;
        var insuranceTypes = _context.TypPojistky.ToList();
        CurrentTableData = new ObservableCollection<object>(insuranceTypes);
    }

    [RelayCommand]
    public void SwitchToPojistnaSmlouva()
    {
        CurrentTableName = MessageContainer.ContractsTableName;
        var contracts = _context.PojistnaSmlouva.ToList();

        CurrentTableData = new ObservableCollection<object>(contracts);
    }






    #endregion

    #region Search Commands
    [RelayCommand]
    public void ClearSearch()
    {
        SearchQuery = string.Empty;
        SwitchToCurrentTable();
    }

    partial void OnSearchQueryChanged(string? value)
    {
        if (string.IsNullOrWhiteSpace(SearchQuery))
        {
            SwitchToCurrentTable();
            return;
        }

        var searchTerm = SearchQuery.ToLower().Trim();
        switch (CurrentTableName)
        {
            case MessageContainer.KlientiTableName:
                SearchKlienti(searchTerm);
                break;
            case MessageContainer.AdresyTableName:
                SearchAdresy(searchTerm);
                break;
            case MessageContainer.PobockyTableName:
                SearchPobocky(searchTerm);
                break;
            case MessageContainer.TypPojistkyTableName:
                SearchTypyPojisty(searchTerm);
                break;
            case MessageContainer.ContractsTableName:
                SearchContracts(searchTerm);
                break;


            default:
                SwitchToCurrentTable();
                break;
        }
    }


    private void SearchKlienti(string searchTerm)
    {
        var filteredClients = _context.Klienti
            .Include(k => k.Adresa)
            .Where(k =>
                (k.Jmeno != null && EF.Functions.Like(k.Jmeno.ToLower(), $"%{searchTerm}%")) ||
                (k.Prijmeni != null && EF.Functions.Like(k.Prijmeni.ToLower(), $"%{searchTerm}%")) ||
                (k.Email != null && EF.Functions.Like(k.Email.ToLower(), $"%{searchTerm}%")) ||
                (k.Telefon != null && EF.Functions.Like(k.Telefon, $"%{searchTerm}%")) ||
                (k.Adresa != null && k.Adresa.Ulice != null && EF.Functions.Like(k.Adresa.Ulice.ToLower(), $"%{searchTerm}%")) ||
                (k.Adresa != null && k.Adresa.Mesto != null && EF.Functions.Like(k.Adresa.Mesto.ToLower(), $"%{searchTerm}%")) ||
                (k.Adresa != null && k.Adresa.Stat != null && EF.Functions.Like(k.Adresa.Stat.ToLower(), $"%{searchTerm}%")))
            .ToList();
        CurrentTableData = new ObservableCollection<object>(filteredClients);
    }

    private void SearchAdresy(string searchTerm)
    {
        var filteredAddresses = _context.Adresy
            .Where(a =>
                (a.Ulice != null && EF.Functions.Like(a.Ulice.ToLower(), $"%{searchTerm}%")) ||
                (a.Mesto != null && EF.Functions.Like(a.Mesto.ToLower(), $"%{searchTerm}%")) ||
                (a.Stat != null && EF.Functions.Like(a.Stat.ToLower(), $"%{searchTerm}%")) ||
                (a.CisloPopisne != null && EF.Functions.Like(a.CisloPopisne.ToLower(), $"%{searchTerm}%")) ||
                (a.PSC != null && EF.Functions.Like(a.PSC.ToLower(), $"%{searchTerm}%")))
            .ToList();
        CurrentTableData = new ObservableCollection<object>(filteredAddresses);
    }

    private void SearchPobocky(string searchTerm)
    {
        var filteredBranches = _context.Pobocky
            .Where(p =>
                (p.Nazev != null && EF.Functions.Like(p.Nazev.ToLower(), $"%{searchTerm}%")) ||
                (p.Telefon != null && EF.Functions.Like(p.Telefon.ToLower(), $"%{searchTerm}%")))
            .ToList();
        CurrentTableData = new ObservableCollection<object>(filteredBranches);
    }



    private void SearchTypyPojisty(string searchTerm)
    {
        try
        {
            var filteredInsuranceTypes = _context.TypPojistky.ToList();

            var lowerTerm = searchTerm.ToLower();

            filteredInsuranceTypes = filteredInsuranceTypes
                .Where(t =>
                    (t.Dostupnost != null && t.Dostupnost.ToLower().Contains(lowerTerm)) ||
                    (t.Podminky != null && t.Podminky.ToLower().Contains(lowerTerm)) ||
                    (t.Popis != null && t.Popis.ToLower().Contains(lowerTerm)) ||
                    t.MaximalneKryti.ToString().Contains(searchTerm) ||
                    t.MinimalneKryti.ToString().Contains(searchTerm) ||
                    t.DatimZacatku.ToString("yyyy-MM-dd").Contains(searchTerm))
                .ToList();

            CurrentTableData = new ObservableCollection<object>(filteredInsuranceTypes);


           
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred during search: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    private void SearchContracts(string searchTerm)
    {
        try
        {
            
            var filteredContracts = _context.PojistnaSmlouva.ToList();

           
            var lowerTerm = searchTerm.ToLower();

            
            filteredContracts = filteredContracts
                .Where(c =>
                    c.PojistnaCastka.ToString().Contains(searchTerm) || 
                    c.Cena.ToString().Contains(searchTerm) || 
                    c.KlientId.ToString().Contains(searchTerm) || 
                    c.PobockyId.ToString().Contains(searchTerm) || 
                    c.TypPojistkyId.ToString().Contains(searchTerm) || 
                    c.DatumZacatkuPlatnosti.ToString("d", System.Globalization.CultureInfo.InvariantCulture).Contains(searchTerm) || 
                    c.DatumUkonceniPlatnosti.ToString("d", System.Globalization.CultureInfo.InvariantCulture).Contains(searchTerm) || 
                    c.DataVystaveni.ToString("d", System.Globalization.CultureInfo.InvariantCulture).Contains(searchTerm)) 
                .ToList();

           
            CurrentTableData = new ObservableCollection<object>(filteredContracts);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred during search: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }







    #endregion

    #region Button Commands
    [RelayCommand]
    public void AddItem()
    {
        Window addView;
        switch (CurrentTableName)
        {
            case MessageContainer.KlientiTableName:
                addView = new AddClientView();
                break;
            case MessageContainer.AdresyTableName:
                addView = new AddAddressView();
                break;
            case MessageContainer.PobockyTableName:
                addView = new AddBranchView(_context);
                break;
            case MessageContainer.TypPojistkyTableName:
                addView = new AddInsuranceTypeView(_context);
                break;
            case MessageContainer.ContractsTableName:
                addView = new AddContractView(_context);
                break;

            default:
                MessageBoxDisplayer.ShowInfo(MessageContainer.AddFunctionalityNotSupported);
                return;
        }
        addView.ShowDialog();
        SwitchToCurrentTable();
    }



    [RelayCommand]
    public void DeleteItem()
    {
        if (SelectedItem == null)
        {
            MessageBoxDisplayer.ShowError(MessageContainer.DeleteItemNotSelected);
            return;
        }

        if (SelectedItem is Klient selectedClient)
            HandleClientDeletion(selectedClient);
        else if (SelectedItem is Adresa selectedAddress)
            HandleAddressDeletion(selectedAddress);
        else if (SelectedItem is Pobocka selectedBranch)
            HandleBranchDeletion(selectedBranch);
        else if (SelectedItem is TypPojistky selectedInsuranceType)
            HandleInsuranceTypeDeletion(selectedInsuranceType);
        else if (SelectedItem is PojistnaSmlouva selectedContract)
            HandleContractDeletion(selectedContract);
        else
            MessageBoxDisplayer.ShowInfo(MessageContainer.DeleteFunctionalityNotSupported);
    }


    [RelayCommand]
    public void Logout()
    {
        new LoginView().Show();
        CloseRegisterWindow();
    }
    #endregion

    #region Tables Deletion Methods
    private void HandleClientDeletion(Klient selectedClient)
    {
        var result = MessageBoxDisplayer.ShowClientDeletionConfirmation(selectedClient.Jmeno!, selectedClient.Prijmeni!);
        if (result != MessageBoxResult.Yes)
            return;

        using var transaction = _context.Database.BeginTransaction();
        try
        {
            var clientIdParam = new Oracle.ManagedDataAccess.Client.OracleParameter("p_id", selectedClient.IdKlientu);
            _context.Database.ExecuteSqlRaw("BEGIN DELETE_CLIENT(:p_id); END;", clientIdParam);
            transaction.Commit();

            MessageBoxDisplayer.ShowInfo(MessageContainer.DeleteClientSuccess);
            SwitchToCurrentTable();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            var detailedMessage = ex.InnerException?.Message ?? ex.Message;
            MessageBoxDisplayer.ShowError(MessageContainer.GetUnexpectedErrorMessage(detailedMessage));
        }
    }

    private void HandleAddressDeletion(Adresa selectedAddress)
    {
        var result = MessageBoxDisplayer.ShowAddressDeletionConfirmation(selectedAddress.Ulice!, selectedAddress.Mesto!);
        if (result != MessageBoxResult.Yes)
            return;

        using var transaction = _context.Database.BeginTransaction();
        try
        {
            var addressIdParam = new Oracle.ManagedDataAccess.Client.OracleParameter("p_id_adresa", selectedAddress.IdAdresa);
            _context.Database.ExecuteSqlRaw("BEGIN DELETE_ADDRESS(:p_id_adresa); END;", addressIdParam);
            transaction.Commit();

            MessageBoxDisplayer.ShowInfo(MessageContainer.DeleteAddressSuccess);
            SwitchToCurrentTable();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            var detailedMessage = ex.InnerException?.Message ?? ex.Message;
            MessageBoxDisplayer.ShowError(MessageContainer.GetUnexpectedErrorMessage(detailedMessage));
        }
    }

    private void HandleBranchDeletion(Pobocka selectedBranch)
    {
        var result = MessageBoxDisplayer.ShowBranchDeletionConfirmation(selectedBranch.Nazev!, selectedBranch.Telefon!);
        if (result != MessageBoxResult.Yes)
            return;

        using var transaction = _context.Database.BeginTransaction();
        try
        {
            var branchIdParam = new Oracle.ManagedDataAccess.Client.OracleParameter("p_ID_POBOCKY", selectedBranch.IdPobocky);
            _context.Database.ExecuteSqlRaw("BEGIN DeletePobocka(:p_ID_POBOCKY); END;", branchIdParam);
            transaction.Commit();

            MessageBoxDisplayer.ShowInfo("Branch deleted successfully.");
            SwitchToCurrentTable();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            var detailedMessage = ex.InnerException?.Message ?? ex.Message;
            MessageBoxDisplayer.ShowError($"An error occurred: {detailedMessage}");
        }
    }

    private void HandleInsuranceTypeDeletion(TypPojistky selectedInsuranceType)
    {
        var result = MessageBoxDisplayer.ShowInsuranceTypeDeletionConfirmation(selectedInsuranceType.Popis!);
        if (result != MessageBoxResult.Yes)
            return;

        using var transaction = _context.Database.BeginTransaction();
        try
        {
            var insuranceTypeIdParam = new Oracle.ManagedDataAccess.Client.OracleParameter("p_ID_TYP", selectedInsuranceType.IdTyp);
            _context.Database.ExecuteSqlRaw("BEGIN DeleteTypPojistky(:p_ID_TYP); END;", insuranceTypeIdParam);
            transaction.Commit();

            MessageBoxDisplayer.ShowInfo("Insurance type deleted successfully.");
            SwitchToCurrentTable();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            var detailedMessage = ex.InnerException?.Message ?? ex.Message;
            MessageBoxDisplayer.ShowError($"An error occurred: {detailedMessage}");
        }
    }

    private void HandleContractDeletion(PojistnaSmlouva selectedContract)
    {
        var result = MessageBoxDisplayer.ShowContractDeletionConfirmation(selectedContract.IdPojistky.ToString());
        if (result != MessageBoxResult.Yes)
            return;

        using var transaction = _context.Database.BeginTransaction();
        try
        {
            var contractIdParam = new Oracle.ManagedDataAccess.Client.OracleParameter("p_ID_POJISTKY", selectedContract.IdPojistky);
            _context.Database.ExecuteSqlRaw("BEGIN DeletePojistnaMlouva(:p_ID_POJISTKY); END;", contractIdParam);
            transaction.Commit();

            MessageBoxDisplayer.ShowInfo("Contract deleted successfully.");
            SwitchToCurrentTable();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            var detailedMessage = ex.InnerException?.Message ?? ex.Message;
            MessageBoxDisplayer.ShowError($"An error occurred: {detailedMessage}");
        }
    }







    #endregion

    #region Private Helper Methods
    private void CloseRegisterWindow()
    {
        Application.Current.Windows
            .OfType<Window>()
            .SingleOrDefault(w => w.DataContext == this)?.Close();
    }
    #endregion
}
