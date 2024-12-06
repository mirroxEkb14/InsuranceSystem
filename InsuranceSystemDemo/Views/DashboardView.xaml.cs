#region Imports
using CommunityToolkit.Mvvm.ComponentModel;
using InsuranceSystemDemo.Controls;
using InsuranceSystemDemo.Database;
using InsuranceSystemDemo.Models;
using InsuranceSystemDemo.Utils;
using InsuranceSystemDemo.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
#endregion

namespace InsuranceSystemDemo.Views
{
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
            if (e.PropertyName == "AdresaId" || e.PropertyName == "IdKlientu" || e.PropertyName == "Adresa" || e.PropertyName == "IdAdresa")
            {
                e.Cancel = true;
            }
        }

        private void MainDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        #region DataGrid Cell Editing Logic

        private void MainDataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            _originalItem = CloneItem(e.Row.Item);
        }

        private void MainDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (_isProcessingEdit) return;

            try
            {
                _isProcessingEdit = true;

                if (e.EditAction == DataGridEditAction.Commit)
                {
                    MainDataGrid.CommitEdit(DataGridEditingUnit.Row, true);

                    var result = MessageBoxDisplayer.ShowDataGridCellEditingSaveChanges();
                    if (result == MessageBoxResult.Yes)
                    {
                        SaveChanges(e.Row.Item); 
                    }
                    else
                    {
                        RestoreOriginalItem(e.Row.Item);
                    }
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
                    context.Klienti.Update(klient);
                }
                else if (editedItem is Adresa adresa)
                {
                    context.Adresy.Update(adresa);
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

     
     ((DashboardViewModel)DataContext).SwitchToCurrentTable();
        }



        private object CloneItem(object item)
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
            throw new ArgumentException("Unsupported type for cloning");
        }

        #endregion
    }
}
