using InsuranceSystemDemo.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace InsuranceSystemDemo.Views
{
    public partial class UserDashboardView : Window
    {
        public UserDashboardView(string currentUsername)
        {
            InitializeComponent();
            DataContext = new UserDashboardViewModel(currentUsername);

            
            MainDataGrid.AutoGeneratingColumn += MainDataGrid_AutoGeneratingColumn;
        }

        private void MainDataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            
            if (e.PropertyName == "IdPojistky")
            {
                e.Column.Header = "Kod Pojistky";
            }
        }
    }
}
