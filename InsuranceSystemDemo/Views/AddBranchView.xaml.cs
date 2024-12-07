#region Imports
using InsuranceSystemDemo.Database;
using InsuranceSystemDemo.ViewModels;
using System.Windows;
#endregion

namespace InsuranceSystemDemo.Views;
public partial class AddBranchView : Window
{
    public AddBranchView(DatabaseContext context)
    {
        InitializeComponent();
        DataContext = new AddBranchViewModel(context);
    }
}
