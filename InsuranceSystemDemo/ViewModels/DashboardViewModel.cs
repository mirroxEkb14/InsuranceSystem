#region Imports
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InsuranceSystemDemo.Controls;
using InsuranceSystemDemo.Database;
using InsuranceSystemDemo.Utils;
using InsuranceSystemDemo.Views;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
#endregion

namespace InsuranceSystemDemo.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    [ObservableProperty] private ObservableCollection<object>? _currentTableData;
    [ObservableProperty] private string? _searchQuery;
    [ObservableProperty] private string _currentTableName;

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
            case MessageContainer.PoliciesTableName:
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
    public void SwitchToPolicies()
    {
        
    }

    [RelayCommand]
    public void SwitchToClaims()
    {
        
    }

    [RelayCommand]
    public void SwitchToPayments()
    {
        
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
            case MessageContainer.PoliciesTableName:
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
        
    }

    [RelayCommand]
    public void Logout()
    {
        new LoginView().Show();
        CloseRegisterWindow();
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
