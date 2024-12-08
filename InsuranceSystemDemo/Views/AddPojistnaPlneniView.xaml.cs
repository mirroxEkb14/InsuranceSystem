#region Imports
using InsuranceSystemDemo.Database;
using InsuranceSystemDemo.Utils;
using InsuranceSystemDemo.ViewModels;
using System.Windows;
#endregion

namespace InsuranceSystemDemo.Views;

public partial class AddPojistnaPlneniView : Window
{
    public AddPojistnaPlneniView()
    {
        InitializeComponent();
        DataContext = new AddPojistnaPlneniViewModel(
            new DatabaseContext(DatabaseContextGetter.GetDatabaseContextOptions()));
    }
}
