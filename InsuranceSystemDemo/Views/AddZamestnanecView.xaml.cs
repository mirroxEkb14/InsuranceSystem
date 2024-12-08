using System.Windows;
using InsuranceSystemDemo.ViewModels;

namespace InsuranceSystemDemo.Views;

public partial class AddZamestnanecView : Window
{
    public AddZamestnanecView()
    {
        InitializeComponent();
        DataContext = new AddZamestnanecViewModel(Close);
    }
}
