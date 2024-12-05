#region Imports
using InsuranceSystemDemo.Controls;
using InsuranceSystemDemo.Database;
using InsuranceSystemDemo.Models;
using InsuranceSystemDemo.Utils;
using InsuranceSystemDemo.ViewModels;
using System.Windows;
using System.Windows.Controls;
#endregion

namespace InsuranceSystemDemo.Views;

public partial class DashboardView : Window
{
    private object? _originalItem = null; 
    private bool _isProcessingEdit = false; 

    //
    // Summary:
    //     Creates «DatabaseContext» options.
    //     Sets the «DataContext» with the passed parameter.
    public DashboardView()
    {
        InitializeComponent();

        var options = DatabaseContextGetter.GetDatabaseContextOptions();
        var context = new DatabaseContext(options);
        DataContext = new DashboardViewModel(context);
    }

    //
    // Summary:
    //     Prevents generating useless columns such as the «Adresa» column for the «Klienti» table.
    private void MainDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
    {
        if (e.PropertyName == "Adresa")
            e.Cancel = true;
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

                // Спрашиваем пользователя, хочет ли он сохранить изменения
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
            context.Klienti.Update((Klient)editedItem);
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

           
            MainDataGrid.Items.Refresh();
        }
    }

  
    private object CloneItem(object item)
    {
        return item switch
        {
            Klient klient => new Klient
            {
                IdKlientu = klient.IdKlientu,
                Jmeno = klient.Jmeno,
                Prijmeni = klient.Prijmeni,
                Email = klient.Email,
                Telefon = klient.Telefon,
                AdresaId = klient.AdresaId,
                Adresa = klient.Adresa
            },
            _ => null
        };
    }

    #endregion
}
