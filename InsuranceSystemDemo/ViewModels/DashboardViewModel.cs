#region Imports
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InsuranceSystemDemo.Database;
using InsuranceSystemDemo.Models;
using InsuranceSystemDemo.Utils;
using InsuranceSystemDemo.Views;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
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
