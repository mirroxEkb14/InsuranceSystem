#region Imports
using InsuranceSystemDemo.Database;
using InsuranceSystemDemo.Utils;
using InsuranceSystemDemo.ViewModels;
using System.Windows;
#endregion

namespace InsuranceSystemDemo.Views;

public partial class AddFakturaView : Window
{
    public AddFakturaView()
    {
        InitializeComponent();
        DataContext = new AddFakturaViewModel(
            new DatabaseContext(DatabaseContextGetter.GetDatabaseContextOptions()));
    }
}
