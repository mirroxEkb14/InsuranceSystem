#region Imports
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InsuranceSystemDemo.Database;
using InsuranceSystemDemo.Models;
using InsuranceSystemDemo.Utils;
using InsuranceSystemDemo.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using Oracle.ManagedDataAccess.Client;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using ClosedXML.Excel;
using System.Globalization;
using System.Windows.Controls;
using System.Data;

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

    public void MainDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
    {
        if (e.PropertyName.Contains("id", StringComparison.OrdinalIgnoreCase))
        {
            e.Cancel = true;
        }
    }

    public void MainDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
    {
        e.Row.Header = (e.Row.GetIndex() + 1).ToString();
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
            case MessageContainer.TypPojistkyTableName:
                SwitchToTypPojistky();
                break;
            case MessageContainer.ContractsTableName:
                SwitchToPojistnaSmlouva(); 
                break;
            case MessageContainer.ZamestnanecTableName:
                SwitchToZamestnanec();
                break;
            case MessageContainer.PohledavkaTableName:
                SwitchToPohledavka();
                break;
            case MessageContainer.ZavazekTableName:
                SwitchToZavazek();
                break;
            case MessageContainer.PojistnaPlneniTableName:
                SwitchToPojistnaPlneni();
                break;
            case MessageContainer.PlatbaTableName:
                SwitchToPlatba();
                break;
            case MessageContainer.HotovostTableName:
                SwitchToHotovost();
                break;
            case MessageContainer.KartaTableName:
                SwitchToKarta();
                break;
            case MessageContainer.FakturaTableName:
                SwitchToFaktura();
                break;
            default:
                CurrentTableData = [];
                break;
        }
    }

    #region Switch Tables Commands



    #region Views
    [RelayCommand]
    public void LoadPaymentsForSelectedClient()
    {
        if (SelectedItem is not Klient selectedClient || selectedClient == null)
            return;

        var payments = _context.PlatbyView
            .Where(p => p.KlientId == selectedClient.IdKlientu)
            .ToList();

        if (payments.Count == 0)
        {
            MessageBox.Show($"No payments found for client: {selectedClient.Prijmeni}",
                            "No Data", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        CurrentTableName = $"Payments for Client ID: {selectedClient.IdKlientu}";
        CurrentTableData = new ObservableCollection<object>(payments);
    }







    [RelayCommand]
    public void LoadZavazkyForSelectedClient()
    {
        if (SelectedItem is not Klient selectedClient || selectedClient == null)
        {
            MessageBox.Show("Please select a client.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var zavazky = _context.ZavazkyView
            .Where(z => z.KlientId == selectedClient.IdKlientu)
            .ToList();

        if (zavazky.Count == 0)
        {
            MessageBox.Show($"No debts found for client: {selectedClient.Jmeno} {selectedClient.Prijmeni}.",
                            "No Data", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        CurrentTableName = $"Debts for Client: {selectedClient.Jmeno} {selectedClient.Prijmeni}";
        CurrentTableData = new ObservableCollection<object>(zavazky);
    }






    [RelayCommand]
    public void LoadActiveContractsForSelectedClient()
    {
        if (SelectedItem is not Klient selectedClient || selectedClient == null)
        {
            MessageBox.Show("Please select a client.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var activeContracts = _context.AktivniSmlouvyView
            .Where(c => c.KlientId == selectedClient.IdKlientu)
            .ToList();

        if (activeContracts.Count == 0)
        {
            MessageBox.Show($"No active contracts found for client: {selectedClient.Jmeno} {selectedClient.Prijmeni}.",
                            "No Data", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        CurrentTableName = $"Active Contracts for Client: {selectedClient.Jmeno} {selectedClient.Prijmeni}";
        CurrentTableData = new ObservableCollection<object>(activeContracts);
    }




    #endregion


    #region Funkce

    [RelayCommand]
    public void LoadTotalPaymentsForSelectedClient()
    {
        if (SelectedItem is not Klient selectedClient || selectedClient == null)
        {
            MessageBox.Show("Please select a client.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var clientId = selectedClient.IdKlientu;

        var totalPayments = _context.PlatbyView
            .FromSqlInterpolated($"SELECT GET_TOTAL_PAYMENTS_FOR_CLIENT({clientId}) AS SUMA_PLATBY FROM DUAL")
            .Select(x => x.SumaPlatby)
            .FirstOrDefault();

        if (totalPayments == 0)
        {
            MessageBox.Show($"No payments found for client: {selectedClient.Jmeno} {selectedClient.Prijmeni}.",
                            "No Data", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var culture = new CultureInfo("cs-CZ");
        MessageBox.Show($"Total payments for client {selectedClient.Jmeno} {selectedClient.Prijmeni}: {totalPayments.ToString("C", culture)}",
                        "Total Payments", MessageBoxButton.OK, MessageBoxImage.Information);
    }



    [RelayCommand]
    public void LoadLastActiveContractDateForSelectedClient()
    {
        if (SelectedItem is not Klient selectedClient || selectedClient == null)
        {
            MessageBox.Show("Please select a client.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var clientId = selectedClient.IdKlientu;

        DateTime? lastContractDate;

        using (var command = _context.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = "SELECT GET_LAST_ACTIVE_CONTRACT_DATE(:clientId) FROM DUAL";
            command.CommandType = System.Data.CommandType.Text;

            var parameter = command.CreateParameter();
            parameter.ParameterName = "clientId";
            parameter.Value = clientId;
            command.Parameters.Add(parameter);

            _context.Database.OpenConnection();

            var result = command.ExecuteScalar();
            lastContractDate = result == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(result);
        }

        if (lastContractDate == null)
        {
            MessageBox.Show($"No active contracts found for client: {selectedClient.Jmeno} {selectedClient.Prijmeni}.",
                            "No Data", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        MessageBox.Show($"Last active contract date for client {selectedClient.Jmeno} {selectedClient.Prijmeni}: {lastContractDate.Value.ToShortDateString()}",
                        "Last Active Contract", MessageBoxButton.OK, MessageBoxImage.Information);
    }



    [RelayCommand]
    public void LoadExpiredContractsForSelectedClient()
    {
        if (SelectedItem is not Klient selectedClient || selectedClient == null)
        {
            MessageBox.Show("Please select a client.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var clientId = selectedClient.IdKlientu;

        var expiredContracts = _context.Set<ExpiredContractsView>()
     .FromSqlInterpolated($"SELECT * FROM V_EXPIRED_CONTRACTS WHERE KLIENT_ID_KLIENTU = {clientId}")
     .ToList();

        if (expiredContracts.Count == 0)
        {
            MessageBox.Show($"No expired contracts found for client: {selectedClient.Jmeno} {selectedClient.Prijmeni}.",
                            "No Expired Contracts", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        CurrentTableName = $"Expired Contracts for Client: {selectedClient.Jmeno} {selectedClient.Prijmeni}";
        CurrentTableData = new ObservableCollection<object>(expiredContracts);

    }








    #endregion



    #region Procedury

    [RelayCommand]
    public void LoadTopClients()
    {
        try
        {
            decimal debtThreshold = 0;   
            decimal paymentLimit = 99999999; 

            var topClients = _context.TopClients
                .FromSqlRaw("BEGIN GetClientsDebtAndPayments(:p1, :p2, :p3); END;",
                            new OracleParameter("p1", debtThreshold),
                            new OracleParameter("p2", paymentLimit),
                            new OracleParameter("p3", OracleDbType.RefCursor, ParameterDirection.Output))
                .ToList();

            if (topClients.Count == 0)
            {
                MessageBox.Show("No clients found with the specified criteria.",
                                "No Data", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            CurrentTableName = "Top Clients";
            CurrentTableData = new ObservableCollection<object>(topClients.Cast<object>());

        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }



    [RelayCommand]
    public void LoadTopClientsByContracts()
    {
        try
        {
            var topClients = _context.TopClientsByContracts
                .FromSqlRaw("BEGIN GetTopClientsByContractCount(:p1); END;",
                            new OracleParameter("p1", OracleDbType.RefCursor, ParameterDirection.Output))
                .ToList();

            if (topClients.Count == 0)
            {
                MessageBox.Show("No clients found.", "No Data", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            CurrentTableName = "Top Clients By Contracts";
            CurrentTableData = new ObservableCollection<object>(
    topClients.Cast<object>()
);

        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }



    [RelayCommand]
    public void LoadEmployeesWithoutSubordinates()
    {
        try
        {
            var employees = _context.EmployeesWithoutSubordinates
                .FromSqlRaw("BEGIN GetEmployeesWithoutSubordinates(:p1); END;",
                            new OracleParameter("p1", OracleDbType.RefCursor, ParameterDirection.Output))
                .ToList();

            if (employees.Count == 0)
            {
                MessageBox.Show("No employees without subordinates found.", "No Data", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            CurrentTableName = "Employees Without Subordinates";
            CurrentTableData = new ObservableCollection<object>(employees.Cast<object>());
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    public void LoadTop5OldestContracts()
    {
        try
        {
            var oldestContracts = _context.Top5OldestContracts
                .FromSqlRaw("BEGIN GetTop5EmployeesWithOldestContracts(:p1); END;",
                            new OracleParameter("p1", OracleDbType.RefCursor, ParameterDirection.Output))
                .ToList();

            if (oldestContracts.Count == 0)
            {
                MessageBox.Show("No records found.", "No Data", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            CurrentTableName = "Top 5 Oldest Contracts";
            CurrentTableData = new ObservableCollection<object>(oldestContracts.Cast<object>());
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }








    #endregion

    #region Exp Cursor
    [RelayCommand]
    public void LoadClientsWithoutActiveContracts()
    {
        try
        {
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "GetClientsWithoutActiveContracts";
                command.CommandType = CommandType.StoredProcedure;

                var p_cursor = new OracleParameter
                {
                    ParameterName = "p_cursor",
                    OracleDbType = OracleDbType.RefCursor,
                    Direction = ParameterDirection.Output
                };

                command.Parameters.Add(p_cursor);

                _context.Database.OpenConnection();

                using (var reader = command.ExecuteReader())
                {
                    var clients = new List<object>();

                    while (reader.Read())
                    {
                        var client = new
                        {
                            Jmeno = reader.GetString(1),
                            Prijmeni = reader.GetString(2)
                        };
                        clients.Add(client);
                    }

                    if (clients.Count == 0)
                    {
                        MessageBox.Show("No clients found without active contracts.",
                                        "No Data", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }

                    CurrentTableName = "Clients Without Active Contracts";
                    CurrentTableData = new ObservableCollection<object>(clients);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }












    #endregion

    #region DOtaz
    [RelayCommand]
    public void LoadZamestnanciHierarchy()
    {
        var hierarchyData = _context.Set<ZamestnanecHierarchyResult>()
            .FromSqlRaw(@"
            SELECT 
                LPAD(' ', LEVEL * 4, ' ') || JMENO || ' ' || PRIJMENI AS EMP_HIER,
                ID_ZAMESTNANCE, 
                MANAGER_ID, 
                POBOCKY_ID_POBOCKY, 
                LEVEL AS CONNECT_BY_LEVEL
            FROM ZAMESTNANEC
            START WITH MANAGER_ID IS NULL
            CONNECT BY PRIOR ID_ZAMESTNANCE = MANAGER_ID
            ORDER SIBLINGS BY JMENO")
            .ToList();

        Console.WriteLine($"Rows returned: {hierarchyData.Count}");
        foreach (var item in hierarchyData)
        {
            Console.WriteLine($"Name: {item.FullNameHierarchy}, ID: {item.IdZamestnance}");
        }

        if (hierarchyData.Count == 0)
        {
            MessageBox.Show("Не найдено ни одного сотрудника в иерархии.",
                            "Нет данных",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
            return;
        }

        CurrentTableName = "Иерархия сотрудников";
        CurrentTableData = new ObservableCollection<object>(hierarchyData);
    }







    #endregion



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
        var contracts = _context.PojistneSmlouvy.ToList();
        CurrentTableData = new ObservableCollection<object>(contracts);
    }

    [RelayCommand]
    public void SwitchToZamestnanec()
    {
        CurrentTableName = MessageContainer.ZamestnanecTableName;
        var zamestnanci = _context.Zamestnanci
            .Include(z => z.Pobocka) 
            .Include(z => z.Adresa)  
            .ToList();
        CurrentTableData = new ObservableCollection<object>(zamestnanci);
    }

    [RelayCommand]
    public void SwitchToPohledavka()
    {
        CurrentTableName = MessageContainer.PohledavkaTableName;
        var pohledavky = _context.Pohledavky
            .Include(p => p.PojistnaSmlouva)
            .ToList();
        CurrentTableData = new ObservableCollection<object>(pohledavky);
    }

    [RelayCommand]
    public void SwitchToZavazek()
    {
        CurrentTableName = MessageContainer.ZavazekTableName;
        var zavazky = _context.Zavazky
             .Include(z => z.Pohledavka)
             .ToList();
        CurrentTableData = new ObservableCollection<object>(zavazky);
    }

    [RelayCommand]
    public void SwitchToPojistnaPlneni()
    {
        CurrentTableName = MessageContainer.PojistnaPlneniTableName;
        var pojistnaPlneni = _context.PojistnePlneni
            .Include(p => p.PojistnaSmlouva)
            .Include(p => p.Zavazky)
            .ToList();
        CurrentTableData = new ObservableCollection<object>(pojistnaPlneni);
    }

    [RelayCommand]
    public void SwitchToPlatba()
    {
        CurrentTableName = MessageContainer.PlatbaTableName;
        var platby = _context.Platby
            .Include(p => p.Klient)
            .Include(p => p.PojistnaSmlouva)
            .ToList();
        CurrentTableData = new ObservableCollection<object>(platby);
    }

    [RelayCommand]
    public void SwitchToHotovost()
    {
        CurrentTableName = MessageContainer.HotovostTableName;
        var hotovosti = _context.Hotovosti
            .Include(h => h.Platba)
            .ToList();
        CurrentTableData = new ObservableCollection<object>(hotovosti);
    }

    [RelayCommand]
    public void SwitchToKarta()
    {
        CurrentTableName = MessageContainer.KartaTableName;
        var karty = _context.Karty
            .Include(k => k.Platba)
            .ToList();
        CurrentTableData = new ObservableCollection<object>(karty);
    }

    [RelayCommand]
    public void SwitchToFaktura()
    {
        CurrentTableName = MessageContainer.FakturaTableName;
        var faktury = _context.Faktury
            .Include(f => f.Platba)
            .ToList();
        CurrentTableData = new ObservableCollection<object>(faktury);
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
            case MessageContainer.ZamestnanecTableName:
                SearchZamestnanci(searchTerm);
                break;
            case MessageContainer.PohledavkaTableName:
                SearchPohledavky(searchTerm);
                break;
            case MessageContainer.ZavazekTableName:
                SearchZavazky(searchTerm);
                break;
            case MessageContainer.PojistnaPlneniTableName:
                SearchPojistnePlneni(searchTerm);
                break;
            case MessageContainer.PlatbaTableName:
                SearchPlatby(searchTerm);
                break;
            case MessageContainer.HotovostTableName:
                SearchHotovosti(searchTerm);
                break;
            case MessageContainer.KartaTableName:
                SearchKarty(searchTerm);
                break;
            case MessageContainer.FakturaTableName:
                SearchFaktury(searchTerm);
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
            MessageBoxDisplayer.ShowError(ex.Message);
        }
    }
    private void SearchContracts(string searchTerm)
    {
        try
        {
            var filteredContracts = _context.PojistneSmlouvy.ToList();
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
            MessageBoxDisplayer.ShowError(ex.Message);
        }
    }

    private void SearchZamestnanci(string searchTerm)
    {
        var results = _context.Zamestnanci
            .Where(z =>
                z.Jmeno.ToLower().Contains(searchTerm) ||
                z.Prijmeni.ToLower().Contains(searchTerm) ||
                z.Role.ToLower().Contains(searchTerm) ||
                (z.Email != null && z.Email.ToLower().Contains(searchTerm)) ||
                z.Telefon.ToString().Contains(searchTerm))
            .ToList();
        CurrentTableData = new ObservableCollection<object>(results);
    }

    private void SearchPohledavky(string searchTerm)
    {
        string lowerQuery = searchTerm.ToLower();
        var filteredPohledavky = _context.Pohledavky
            .Include(p => p.PojistnaSmlouva)
            .AsEnumerable()
            .Where(p =>
                p.SumaPohledavky.ToString().ToLower().Contains(lowerQuery) ||
                p.DatumZacatku.ToString("dd.MM.yyyy").ToLower().Contains(lowerQuery) ||
                p.DatumKonce.ToString("dd.MM.yyyy").ToLower().Contains(lowerQuery) ||
                p.PojistnaSmlouvaId.ToString().ToLower().Contains(lowerQuery))
            .ToList();
        CurrentTableData = new ObservableCollection<object>(filteredPohledavky);
    }

    private void SearchZavazky(string searchTerm)
    {
        string lowerQuery = searchTerm.ToLower();
        var filteredZavazky = _context.Zavazky
            .Include(z => z.Pohledavka)
            .AsEnumerable()
            .Where(z =>
                z.SumaZavazky.ToString().ToLower().Contains(lowerQuery) ||
                z.DataVzniku.ToString("dd.MM.yyyy").ToLower().Contains(lowerQuery) ||
                z.DataSplatnisti.ToString("dd.MM.yyyy").ToLower().Contains(lowerQuery) ||
                z.PohledavkaIdPohledavky.ToString().ToLower().Contains(lowerQuery))
            .ToList();
        CurrentTableData = new ObservableCollection<object>(filteredZavazky);
    }

    private void SearchPojistnePlneni(string searchTerm)
    {
        string lowerQuery = searchTerm.ToLower();
        var filteredPojistnePlneni = _context.PojistnePlneni
            .Include(p => p.PojistnaSmlouva)
            .Include(p => p.Zavazky)
            .AsEnumerable()
            .Where(p =>
                p.SumaPlneni.ToString().Contains(lowerQuery) ||
                p.PojistnaSmlouvaIdPojistky.ToString().Contains(lowerQuery) ||
                p.ZavazkyIdZavazky.ToString().Contains(lowerQuery) ||
                (p.PojistnaSmlouva?.IdPojistky.ToString().Contains(lowerQuery) ?? false) ||
                (p.Zavazky?.IdZavazky.ToString().Contains(lowerQuery) ?? false))
            .ToList();
        CurrentTableData = new ObservableCollection<object>(filteredPojistnePlneni);
    }

    private void SearchPlatby(string searchTerm)
    {
        var lowerQuery = searchTerm.ToLower();
        var filteredPlatby = _context.Platby
            .Include(p => p.Klient)
            .Include(p => p.PojistnaSmlouva)
            .AsEnumerable()
            .Where(p =>
                p.SumaPlatby.ToString().ToLower().Contains(lowerQuery) ||
                p.DatumPlatby.ToString("dd.MM.yyyy").ToLower().Contains(lowerQuery) ||
                (p.TypPlatby != null && p.TypPlatby.ToString().ToLower().Contains(lowerQuery)))
            .ToList();
        CurrentTableData = new ObservableCollection<object>(filteredPlatby);
    }

    private void SearchHotovosti(string searchTerm)
    {
        var lowerQuery = searchTerm.ToLower();
        var filteredHotovosti = _context.Hotovosti
            .Where(h => h.Prijato.ToString().Contains(lowerQuery) ||
                        h.Vraceno.ToString().Contains(lowerQuery))
            .ToList();
        CurrentTableData = new ObservableCollection<object>(filteredHotovosti);
    }

    private void SearchKarty(string searchTerm)
    {
        var lowerQuery = searchTerm.ToLower();
        var filteredKarty = _context.Karty
            .Where(k => k.CisloKarty.ToString().Contains(lowerQuery) ||
                        k.CisloUctu.ToString().Contains(lowerQuery))
            .ToList();
        CurrentTableData = new ObservableCollection<object>(filteredKarty);
    }

    private void SearchFaktury(string searchTerm)
    {
        var lowerQuery = searchTerm.ToLower();
        var filteredFaktury = _context.Faktury
            .Include(f => f.Platba)
            .AsEnumerable()
            .Where(f => f.CisloUctu.ToString().Contains(lowerQuery) ||
                        f.DatumSplatnosti.ToString("dd.MM.yyyy").ToLower().Contains(lowerQuery))
            .ToList();
        CurrentTableData = new ObservableCollection<object>(filteredFaktury);
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
            case MessageContainer.ZamestnanecTableName: 
                addView = new AddZamestnanecView(); 
                break;
            case MessageContainer.PohledavkaTableName:
                addView = new AddPohledavkaView();
                break;
            case MessageContainer.ZavazekTableName:
                addView = new AddZavazekView();
                break;
            case MessageContainer.PojistnaPlneniTableName:
                addView = new AddPojistnaPlneniView();
                break;
            case MessageContainer.PlatbaTableName:
                addView = new AddPlatbaView();
                break;
            case MessageContainer.HotovostTableName:
                addView = new AddHotovostView();
                break;
            case MessageContainer.KartaTableName:
                addView = new AddKartaView();
                break;
            case MessageContainer.FakturaTableName:
                addView = new AddFakturaView();
                break;
            default:
                MessageBoxDisplayer.ShowInfo(MessageContainer.AddFunctionalityNotSupported);
                return;
        }
        addView.ShowDialog(); 
        SwitchToCurrentTable(); 
    }



    [RelayCommand]
    public void Download()
    {
        if (CurrentTableData == null || !CurrentTableData.Any())
        {
            MessageBoxDisplayer.ShowInfo("No data to download.");
            return;
        }

       
        SaveFileDialog saveFileDialog = new SaveFileDialog
        {
            Filter = "Excel Files (*.xlsx)|*.xlsx|All Files (*.*)|*.*",
            DefaultExt = ".xlsx",
            FileName = $"{CurrentTableName}_Export"
        };

        if (saveFileDialog.ShowDialog() == true)
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add(CurrentTableName);

                    
                    var itemType = CurrentTableData.First().GetType();
                    var properties = itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                             .Where(p => !p.Name.Contains("Id", StringComparison.OrdinalIgnoreCase)) 
                                             .ToList();

                    
                    for (int col = 0; col < properties.Count; col++)
                    {
                        worksheet.Cell(1, col + 1).Value = properties[col].Name;
                    }

                   
                    int row = 2;
                    foreach (var item in CurrentTableData)
                    {
                        for (int col = 0; col < properties.Count; col++)
                        {
                            worksheet.Cell(row, col + 1).Value = properties[col].GetValue(item)?.ToString() ?? string.Empty;
                        }
                        row++;
                    }

                   
                    workbook.SaveAs(saveFileDialog.FileName);
                }

                MessageBoxDisplayer.ShowInfo("Data successfully exported to Excel.");
            }
            catch (Exception ex)
            {
                MessageBoxDisplayer.ShowError($"An error occurred while exporting data: {ex.Message}");
            }
        }
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
        else if (SelectedItem is Zamestnanec selectedZamestnanec) 
            HandleZamestnanecDeletion(selectedZamestnanec);
        else if (SelectedItem is Pohledavka selectedPohledavka)
            HandlePohledavkaDeletion(selectedPohledavka);
        else if (SelectedItem is Zavazek selectedZavazek)
            HandleZavazekDeletion(selectedZavazek);
        else if (SelectedItem is PojistnaPlneni selectedPojistnaPlneni)
            HandlePojistnaPlneniDeletion(selectedPojistnaPlneni);
        else if (SelectedItem is Platba selectedPlatba)
            HandlePlatbaDeletion(selectedPlatba);
        else if (SelectedItem is Hotovost selectedHotovost)
            HandleHotovostDeletion(selectedHotovost);
        else if (SelectedItem is Karta selectedKarta)
            HandleKartaDeletion(selectedKarta);
        else if (SelectedItem is Faktura selectedFaktura)
            HandleFakturaDeletion(selectedFaktura);
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
            var clientIdParam = new OracleParameter("p_id", selectedClient.IdKlientu);
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
            var addressIdParam = new OracleParameter("p_id_adresa", selectedAddress.IdAdresa);
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
            var branchIdParam = new OracleParameter("p_ID_POBOCKY", selectedBranch.IdPobocky);
            _context.Database.ExecuteSqlRaw("BEGIN DeletePobocka(:p_ID_POBOCKY); END;", branchIdParam);
            transaction.Commit();

            MessageBoxDisplayer.ShowInfo(MessageContainer.DeleteBranchSuccess);
            SwitchToCurrentTable();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            var detailedMessage = ex.InnerException?.Message ?? ex.Message;
            MessageBoxDisplayer.ShowError(MessageContainer.GetUnexpectedErrorMessage(detailedMessage));
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
            var insuranceTypeIdParam = new OracleParameter("p_ID_TYP", selectedInsuranceType.IdTyp);
            _context.Database.ExecuteSqlRaw("BEGIN DeleteTypPojistky(:p_ID_TYP); END;", insuranceTypeIdParam);
            transaction.Commit();

            MessageBoxDisplayer.ShowInfo(MessageContainer.DeleteInsuranceTypeSuccess);
            SwitchToCurrentTable();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            var detailedMessage = ex.InnerException?.Message ?? ex.Message;
            MessageBoxDisplayer.ShowError(MessageContainer.GetUnexpectedErrorMessage(detailedMessage));
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
            var contractIdParam = new OracleParameter("p_ID_POJISTKY", selectedContract.IdPojistky);
            _context.Database.ExecuteSqlRaw("BEGIN DeletePojistnaMlouva(:p_ID_POJISTKY); END;", contractIdParam);
            transaction.Commit();

            MessageBoxDisplayer.ShowInfo(MessageContainer.DeleteContractSuccess);
            SwitchToCurrentTable();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            var detailedMessage = ex.InnerException?.Message ?? ex.Message;
            MessageBoxDisplayer.ShowError(MessageContainer.GetUnexpectedErrorMessage(detailedMessage));
        }
    }

    private void HandleZamestnanecDeletion(Zamestnanec zamestnanec)
    {
        var result = MessageBoxDisplayer.ShowEmployeeDeletionConfirmation(zamestnanec.Jmeno, zamestnanec.Prijmeni);
        if (result != MessageBoxResult.Yes)
            return;

        try
        {
            using var context = new DatabaseContext(DatabaseContextGetter.GetDatabaseContextOptions());
            context.Database.ExecuteSqlRaw(
                "BEGIN DELETEZAMESTNANEC(:p_id_zamestnance); END;",
                new OracleParameter("p_id_zamestnance", zamestnanec.IdZamestnance)
            );

            MessageBoxDisplayer.ShowInfo(MessageContainer.DeleteEmployeeSuccess);
            SwitchToCurrentTable();
        }
        catch (Exception ex)
        {
            MessageBoxDisplayer.ShowError(MessageContainer.GetUnexpectedErrorMessage(ex.Message));
        }
    }

    private void HandlePohledavkaDeletion(Pohledavka pohledavka)
    {
        var result = MessageBoxDisplayer.ShowDebtDeletionConfirmation(pohledavka.SumaPohledavky.ToString("F2"));
        if (result != MessageBoxResult.Yes)
            return;

        try
        {
            using var context = new DatabaseContext(DatabaseContextGetter.GetDatabaseContextOptions());
            context.Database.ExecuteSqlRaw(
                "BEGIN DELETE_POHLEDAVKA(:p_id_pohledavky); END;",
                new OracleParameter("p_id_pohledavky", pohledavka.IdPohledavky)
            );

            MessageBoxDisplayer.ShowInfo(MessageContainer.DeleteDebtSuccess);
            SwitchToCurrentTable();
        }
        catch (Exception ex)
        {
            MessageBoxDisplayer.ShowError(MessageContainer.GetUnexpectedErrorMessage(ex.Message));
        }
    }







    private void HandleZavazekDeletion(Zavazek zavazek)
    {
        var result = MessageBoxDisplayer.ShowBankfillDeletionConfirmation(zavazek.IdZavazky.ToString());
        if (result != MessageBoxResult.Yes)
            return;

        try
        {
            // TODO

            MessageBoxDisplayer.ShowInfo(MessageContainer.DeleteBankfillSuccess);
            SwitchToCurrentTable();
        }
        catch (Exception ex)
        {
            MessageBoxDisplayer.ShowError(MessageContainer.GetUnexpectedErrorMessage(ex.Message));
        }
    }

    private void HandlePojistnaPlneniDeletion(PojistnaPlneni pojistnaPlneni)
    {
        var result = MessageBoxDisplayer.ShowInsuranceFulfilmentDeletionConfirmation(pojistnaPlneni.IdPlneni.ToString());
        if (result != MessageBoxResult.Yes)
            return;

        try
        {
            // TODO

            MessageBoxDisplayer.ShowInfo(MessageContainer.DeleteInsuranceFulfilmentSuccess);
            SwitchToCurrentTable();
        }
        catch (Exception ex)
        {
            MessageBoxDisplayer.ShowError(MessageContainer.GetUnexpectedErrorMessage(ex.Message));
        }
    }

    private void HandlePlatbaDeletion(Platba platba)
    {
        var result = MessageBoxDisplayer.ShowPaymentDeletionConfirmation(platba.IdPlatby.ToString());
        if (result != MessageBoxResult.Yes)
            return;

        try
        {
            // TODO

            MessageBoxDisplayer.ShowInfo(MessageContainer.DeletePaymentSuccess);
            SwitchToCurrentTable();
        }
        catch (Exception ex)
        {
            MessageBoxDisplayer.ShowError(MessageContainer.GetUnexpectedErrorMessage(ex.Message));
        }
    }

    private void HandleHotovostDeletion(Hotovost hotovost)
    {
        var result = MessageBoxDisplayer.ShowCashDeletionConfirmation(hotovost.IdPlatby.ToString());
        if (result != MessageBoxResult.Yes)
            return;

        try
        {
            // TODO

            MessageBoxDisplayer.ShowInfo(MessageContainer.DeleteCashPaymentSuccess);
            SwitchToCurrentTable();
        }
        catch (Exception ex)
        {
            MessageBoxDisplayer.ShowError(MessageContainer.GetUnexpectedErrorMessage(ex.Message));
        }
    }

    private void HandleKartaDeletion(Karta karta)
    {
        var result = MessageBoxDisplayer.ShowCardDeletionConfirmation();
        if (result != MessageBoxResult.Yes)
            return;

        try
        {
            // TODO

            MessageBoxDisplayer.ShowInfo(MessageContainer.DeleteCardPaymentSuccess);
            SwitchToCurrentTable();
        }
        catch (Exception ex)
        {
            MessageBoxDisplayer.ShowError(MessageContainer.GetUnexpectedErrorMessage(ex.Message));
        }
    }

    private void HandleFakturaDeletion(Faktura faktura)
    {
        var result = MessageBoxDisplayer.ShowBillDeletionConfirmation();
        if (result != MessageBoxResult.Yes)
            return;

        try
        {
            // TODO

            MessageBoxDisplayer.ShowInfo(MessageContainer.DeleteBillPaymentSuccess);
            SwitchToCurrentTable();
        }
        catch (Exception ex)
        {
            MessageBoxDisplayer.ShowError(MessageContainer.GetUnexpectedErrorMessage(ex.Message));
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
