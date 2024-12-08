#region Imports
using InsuranceSystemDemo.Database;
using InsuranceSystemDemo.Utils;
using InsuranceSystemDemo.ViewModels;
using System.Windows;
#endregion

namespace InsuranceSystemDemo.Views;

public partial class AddHotovostView : Window
{
    public AddHotovostView()
    {
        InitializeComponent();
        DataContext = new AddHotovostViewModel(
            new DatabaseContext(DatabaseContextGetter.GetDatabaseContextOptions()));
    }
}
