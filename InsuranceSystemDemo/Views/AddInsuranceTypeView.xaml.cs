using System.Windows;
using InsuranceSystemDemo.Database;
using InsuranceSystemDemo.ViewModels;

namespace InsuranceSystemDemo.Views
{
    public partial class AddInsuranceTypeView : Window
    {
        public AddInsuranceTypeView(DatabaseContext context)
        {
            InitializeComponent();
            DataContext = new AddInsuranceTypeViewModel(context);
        }
    }
}
