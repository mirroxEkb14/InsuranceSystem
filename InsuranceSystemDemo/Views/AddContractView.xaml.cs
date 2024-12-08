#region Imports
using InsuranceSystemDemo.ViewModels;
using InsuranceSystemDemo.Database;
using System.Windows;
#endregion

namespace InsuranceSystemDemo.Views;

public partial class AddContractView : Window
{
    public AddContractView(DatabaseContext context)
    {
        InitializeComponent();
        DataContext = new AddContractViewModel(context);
    }
}
