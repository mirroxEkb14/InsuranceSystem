#region Imports
using InsuranceSystemDemo.Utils;
using InsuranceSystemDemo.ViewModels;
using System.Windows;
#endregion

namespace InsuranceSystemDemo.Views;

public partial class AddPohledavkaView : Window
{
    public AddPohledavkaView()
    {
        InitializeComponent();
        DataContext = new AddPohledavkaViewModel(
            new(DatabaseContextGetter.GetDatabaseContextOptions()));
    }
}
