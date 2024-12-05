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
    private void MainDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    {
        if (e.EditAction == DataGridEditAction.Commit)
        {
            var result = MessageBoxDisplayer.ShowDataGridCellEditingSaveChanges();
            if (result == MessageBoxResult.Yes)
                SaveChanges(e.Row.Item);
            else
                ((DashboardViewModel)DataContext).SwitchToCurrentTable();
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
    #endregion
}
