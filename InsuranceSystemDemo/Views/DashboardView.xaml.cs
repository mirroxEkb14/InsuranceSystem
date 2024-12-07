#region Imports
using InsuranceSystemDemo.Database;
using InsuranceSystemDemo.Models;
using InsuranceSystemDemo.Utils;
using InsuranceSystemDemo.ViewModels;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System.Windows;
using System.Windows.Controls;
#endregion

namespace InsuranceSystemDemo.Views;

public partial class DashboardView : Window
{
    private object? _originalItem;
    private bool _isProcessingEdit = false;

    public DashboardView()
    {
        InitializeComponent();

        var options = DatabaseContextGetter.GetDatabaseContextOptions();
        var context = new DatabaseContext(options);
        DataContext = new DashboardViewModel(context);
    }

    private void MainDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
    {
        if (e.PropertyName == "IdTyp" || e.PropertyName == "IdPobocky" ||  e.PropertyName == "AdresaId" || e.PropertyName == "IdKlientu" || e.PropertyName == "Adresa" || e.PropertyName == "IdAdresa")
            e.Cancel = true;
    }

    private void MainDataGrid_LoadingRow(object sender, DataGridRowEventArgs e) =>
        e.Row.Header = (e.Row.GetIndex() + 1).ToString();

    #region DataGrid Cell Editing Logic
    private void MainDataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
    {
        _originalItem = CloneItem(e.Row.Item);
    }

    private void MainDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
    {
        if (_isProcessingEdit)
            return;

        try
        {
            _isProcessingEdit = true;
            if (e.EditAction == DataGridEditAction.Commit)
            {
                MainDataGrid.CommitEdit(DataGridEditingUnit.Row, true);

                var result = MessageBoxDisplayer.ShowDataGridCellEditingSaveChanges();
                if (result == MessageBoxResult.Yes)
                    SaveChanges(e.Row.Item);
                else
                    RestoreOriginalItem(e.Row.Item);
            }
        }
        finally
        {
            _isProcessingEdit = false;
        }
    }

    private void SaveChanges(object editedItem)
    {
        try
        {
            using var context = new DatabaseContext(DatabaseContextGetter.GetDatabaseContextOptions());

            if (editedItem is Klient klient)
            {
                context.Database.ExecuteSqlRaw(
                    "BEGIN UpdateKlient(:p_ID_KLIENTU, :p_JMENO, :p_PRIJMENI, :p_EMAIL, :p_TELEFON, :p_ADRESA_ID_ADRESA, :p_DATUM_NAROZENI); END;",
                    new OracleParameter("p_ID_KLIENTU", klient.IdKlientu),
                    new OracleParameter("p_JMENO", klient.Jmeno),
                    new OracleParameter("p_PRIJMENI", klient.Prijmeni),
                    new OracleParameter("p_EMAIL", klient.Email ?? (object)DBNull.Value),
                    new OracleParameter("p_TELEFON", klient.Telefon),
                    new OracleParameter("p_ADRESA_ID_ADRESA", klient.AdresaId),
                    new OracleParameter("p_DATUM_NAROZENI", klient.DatumNarozeni ?? (object)DBNull.Value)
                );
            }
            else if (editedItem is Adresa adresa)
            {
                context.Database.ExecuteSqlRaw(
                    "BEGIN UpdateAdresa(:p_ID_ADRESA, :p_ULICE, :p_MESTO, :p_STAT, :p_CISLO_POPISNE, :p_PSC); END;",
                    new OracleParameter("p_ID_ADRESA", adresa.IdAdresa),
                    new OracleParameter("p_ULICE", adresa.Ulice),
                    new OracleParameter("p_MESTO", adresa.Mesto),
                    new OracleParameter("p_STAT", adresa.Stat),
                    new OracleParameter("p_CISLO_POPISNE", adresa.CisloPopisne),
                    new OracleParameter("p_PSC", adresa.PSC)
                );
            }
            else if (editedItem is Pobocka pobocka)
            {
                context.Database.ExecuteSqlRaw(
                    "BEGIN UpdatePobocka(:p_ID_POBOCKY, :p_NAZEV, :p_TELEFON, :p_ADRESA_ID_ADRESA); END;",
                    new OracleParameter("p_ID_POBOCKY", pobocka.IdPobocky),
                    new OracleParameter("p_NAZEV", pobocka.Nazev),
                    new OracleParameter("p_TELEFON", pobocka.Telefon),
                    new OracleParameter("p_ADRESA_ID_ADRESA", pobocka.AdresaId)
                );
            }

            context.SaveChanges();
            MessageBoxDisplayer.ShowInfo(MessageContainer.DataGridCellEditingChangesSaved);
        }
        catch (Exception ex)
        {
            MessageBoxDisplayer.ShowError(MessageContainer.GetUnexpectedErrorMessage(ex.Message));
            ((DashboardViewModel)DataContext).SwitchToCurrentTable();
        }
    }




    private void RestoreOriginalItem(object currentItem)
    {
        if (_originalItem is Klient originalKlient && currentItem is Klient editedKlient)
        {
            editedKlient.IdKlientu = originalKlient.IdKlientu;
            editedKlient.Jmeno = originalKlient.Jmeno;
            editedKlient.Prijmeni = originalKlient.Prijmeni;
            editedKlient.Email = originalKlient.Email;
            editedKlient.Telefon = originalKlient.Telefon;
            editedKlient.AdresaId = originalKlient.AdresaId;
            editedKlient.Adresa = originalKlient.Adresa;
        }
        else if (_originalItem is Adresa originalAdresa && currentItem is Adresa editedAdresa)
        {
            editedAdresa.IdAdresa = originalAdresa.IdAdresa;
            editedAdresa.Ulice = originalAdresa.Ulice;
            editedAdresa.Mesto = originalAdresa.Mesto;
            editedAdresa.Stat = originalAdresa.Stat;
            editedAdresa.CisloPopisne = originalAdresa.CisloPopisne;
            editedAdresa.PSC = originalAdresa.PSC;
        }
        else if (_originalItem is Pobocka originalPobocka && currentItem is Pobocka editedPobocka)
        {
            editedPobocka.IdPobocky = originalPobocka.IdPobocky;
            editedPobocka.Nazev = originalPobocka.Nazev;
            editedPobocka.Telefon = originalPobocka.Telefon;
            editedPobocka.AdresaId = originalPobocka.AdresaId;
            editedPobocka.Adresa = originalPobocka.Adresa;
        }
    ((DashboardViewModel)DataContext).SwitchToCurrentTable();
    }


    private static object CloneItem(object item)
    {
        if (item is Klient klient)
        {
            return new Klient
            {
                IdKlientu = klient.IdKlientu,
                Jmeno = klient.Jmeno,
                Prijmeni = klient.Prijmeni,
                Email = klient.Email,
                Telefon = klient.Telefon,
                AdresaId = klient.AdresaId,
                Adresa = klient.Adresa
            };
        }
        else if (item is Adresa adresa)
        {
            return new Adresa
            {
                IdAdresa = adresa.IdAdresa,
                Ulice = adresa.Ulice,
                Mesto = adresa.Mesto,
                Stat = adresa.Stat,
                CisloPopisne = adresa.CisloPopisne,
                PSC = adresa.PSC
            };
        }
        else if (item is Pobocka pobocka)
        {
            return new Pobocka
            {
                IdPobocky = pobocka.IdPobocky,
                Nazev = pobocka.Nazev,
                Telefon = pobocka.Telefon,
                AdresaId = pobocka.AdresaId,
                Adresa = pobocka.Adresa
            };
        }
        throw new ArgumentException("Unsupported type for cloning");
    }

    #endregion
}
