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
    //         the «DataGrid» cell editing.
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
            default:
                CurrentTableData = [];
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
        throw new NotImplementedException();
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
