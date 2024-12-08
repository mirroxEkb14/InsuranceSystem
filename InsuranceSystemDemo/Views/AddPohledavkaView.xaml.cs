using System.Windows;
using InsuranceSystemDemo.ViewModels;

namespace InsuranceSystemDemo.Views;

public partial class AddPohledavkaView : Window
{
    public AddPohledavkaView()
    {
        InitializeComponent();
        DataContext = new AddPohledavkaViewModel(Close);
    }
}

