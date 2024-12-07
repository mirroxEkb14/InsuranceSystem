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
        if (e.PropertyName.Contains("Id", StringComparison.OrdinalIgnoreCase))
        {
            e.Cancel = true;
        }
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
                    "BEGIN UpdateAdresa(:p_ID_ADRESA, :p_ULICE, :p_MESTO, :p_STAT, :p_CISLO_POPISНЕ, :p_PSC); END;",
                    new OracleParameter("p_ID_ADRESA", adresa.IdAdresa),
                    new OracleParameter("p_ULICE", adresa.Ulice),
                    new OracleParameter("p_MESTО", adresa.Mesto),
                    new OracleParameter("p_STAT", adresa.Stat),
                    new OracleParameter("p_CISЛO_POPИСНЕ", adresa.CisloPopisne),
                    new OracleParameter("p_PSC", adresa.PSC)
                );
            }
            else if (editedItem is Pobocka pobocka)
            {
                context.Database.ExecuteSqlRaw(
                    "BEGIN UpdatePobocka(:p_ID_POBOCKY, :p_NAZEV, :p_TELEФОН, :p_ADRESA_ID_ADRESA); END;",
                    new OracleParameter("p_ID_POBOCKY", pobocka.IdPobocky),
                    new OracleParameter("p_NAZEV", pobocka.Nazev),
                    new OracleParameter("p_TELEФОН", pobocka.Telefon),
                    new OracleParameter("p_ADRESA_ID_ADRESA", pobocka.AdresaId)
                );
            }
            else if (editedItem is TypPojistky typPojistky)
            {
                context.Database.ExecuteSqlRaw(
                    "BEGIN UpdateTypPojistky(:p_ID_TYP, :p_DOSTUPNOST, :p_PODMINKY, :p_POPIS, :p_MAXIMALNE_KRYTI, :p_MINIMALNE_KRYTI, :p_DATIM_ZACATKU); END;",
                    new OracleParameter("p_ID_TYP", typPojistky.IdTyp),
                    new OracleParameter("p_DOSTUPNOST", typPojistky.Dostupnost),
                    new OracleParameter("p_PODMINKY", typPojistky.Podminky ?? (object)DBNull.Value),
                    new OracleParameter("p_POPIS", typPojistky.Popis ?? (object)DBNull.Value),
                    new OracleParameter("p_MAXIMALNE_KRYTI", typPojistky.MaximalneKryti),
                    new OracleParameter("p_MINIMALNE_KRYTI", typPojistky.MinimalneKryti),
                    new OracleParameter("p_DATIM_ZACATKU", typPojistky.DatimZacatku)
                );
            }
            else if (editedItem is PojistnaSmlouva contract)
            {
                context.Database.ExecuteSqlRaw(
                    "BEGIN UpdatePojistnaMlouva(:p_ID_POJISTKY, :p_POJISTNA_CASTKA, :p_DATUM_ZACATKU_PLATNOSTI, :p_DATUM_UKONCENI_PLATNOSTI, :p_DATA_VYSTAVENI, :p_CENA, :p_KLIENT_ID_KLIENTU, :p_POBOCKY_ID_POBOCKY, :p_TYPPOJISTKY_ID_TYP); END;",
                    new OracleParameter("p_ID_POJISTKY", contract.IdPojistky),
                    new OracleParameter("p_POJISTNA_CASTKA", contract.PojistnaCastka),
                    new OracleParameter("p_DATUM_ZACATKU_PLATNOSTI", contract.DatumZacatkuPlatnosti),
                    new OracleParameter("p_DATUM_UKONCENI_PLATNOSTI", contract.DatumUkonceniPlatnosti),
                    new OracleParameter("p_DATA_VYSTAVENI", contract.DataVystaveni),
                    new OracleParameter("p_CENA", contract.Cena),
                    new OracleParameter("p_KLIENT_ID_KLIENTU", contract.KlientId),
                    new OracleParameter("p_POBOCKY_ID_POBOCKY", contract.PobockyId),
                    new OracleParameter("p_TYPPOJISTKY_ID_TYP", contract.TypPojistkyId)
                );
            }

            MessageBoxDisplayer.ShowInfo(MessageContainer.DataGridCellEditingChangesSaved);
        }
        catch (Exception ex)
        {
            var errorDetails = new System.Text.StringBuilder();
            errorDetails.AppendLine("An error occurred:");

            var currentException = ex;
            while (currentException != null)
            {
                errorDetails.AppendLine($"Message: {currentException.Message}");
                errorDetails.AppendLine($"StackTrace: {currentException.StackTrace}");
                currentException = currentException.InnerException;
            }

            MessageBox.Show(errorDetails.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Console.WriteLine(errorDetails.ToString());

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
        else if (_originalItem is TypPojistky originalTyp && currentItem is TypPojistky editedTyp)
        {
            editedTyp.IdTyp = originalTyp.IdTyp;
            editedTyp.Dostupnost = originalTyp.Dostupnost;
            editedTyp.Podminky = originalTyp.Podminky;
            editedTyp.Popis = originalTyp.Popis;
            editedTyp.MaximalneKryti = originalTyp.MaximalneKryti;
            editedTyp.MinimalneKryti = originalTyp.MinimalneKryti;
            editedTyp.DatimZacatku = originalTyp.DatimZacatku;
        }
        else if (_originalItem is PojistnaSmlouva originalContract && currentItem is PojistnaSmlouva editedContract)
        {
            editedContract.IdPojistky = originalContract.IdPojistky;
            editedContract.PojistnaCastka = originalContract.PojistnaCastka;
            editedContract.DatumZacatkuPlatnosti = originalContract.DatumZacatkuPlatnosti;
            editedContract.DatumUkonceniPlatnosti = originalContract.DatumUkonceniPlatnosti;
            editedContract.DataVystaveni = originalContract.DataVystaveni;
            editedContract.Cena = originalContract.Cena;
            editedContract.KlientId = originalContract.KlientId;
            editedContract.PobockyId = originalContract.PobockyId;
            editedContract.TypPojistkyId = originalContract.TypPojistkyId;
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
        else if (item is TypPojistky typPojistky)
        {
            return new TypPojistky
            {
                IdTyp = typPojistky.IdTyp,
                Dostupnost = typPojistky.Dostupnost,
                Podminky = typPojistky.Podminky,
                Popis = typPojistky.Popis,
                MaximalneKryti = typPojistky.MaximalneKryti,
                MinimalneKryti = typPojistky.MinimalneKryti,
                DatimZacatku = typPojistky.DatimZacatku
            };
        }
        else if (item is PojistnaSmlouva contract)
        {
            return new PojistnaSmlouva
            {
                IdPojistky = contract.IdPojistky,
                PojistnaCastka = contract.PojistnaCastka,
                DatumZacatkuPlatnosti = contract.DatumZacatkuPlatnosti,
                DatumUkonceniPlatnosti = contract.DatumUkonceniPlatnosti,
                DataVystaveni = contract.DataVystaveni,
                Cena = contract.Cena,
                KlientId = contract.KlientId,
                PobockyId = contract.PobockyId,
                TypPojistkyId = contract.TypPojistkyId
            };
        }
        throw new ArgumentException("Unsupported type for cloning");
    }



    #endregion
}
