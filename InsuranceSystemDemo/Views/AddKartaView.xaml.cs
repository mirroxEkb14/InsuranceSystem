#region Imports
using InsuranceSystemDemo.Database;
using InsuranceSystemDemo.Utils;
using InsuranceSystemDemo.ViewModels;
using System.Windows;
#endregion

namespace InsuranceSystemDemo.Views;

public partial class AddKartaView : Window
{
    public AddKartaView()
    {
        InitializeComponent();
        DataContext = new AddKartaViewModel(
            new DatabaseContext(DatabaseContextGetter.GetDatabaseContextOptions()));
    }
}
