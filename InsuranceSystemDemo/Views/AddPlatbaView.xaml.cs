#region Imports
using InsuranceSystemDemo.Database;
using InsuranceSystemDemo.Utils;
using InsuranceSystemDemo.ViewModels;
using System.Windows;
#endregion

namespace InsuranceSystemDemo.Views;

public partial class AddPlatbaView : Window
{
    public AddPlatbaView()
    {
        InitializeComponent();
        DataContext = new AddPlatbaViewModel(
            new DatabaseContext(DatabaseContextGetter.GetDatabaseContextOptions()));
    }
}
