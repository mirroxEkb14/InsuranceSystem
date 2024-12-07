#region Imports
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
#endregion

namespace InsuranceSystemDemo.ViewModels;

//
// Summary:
//     Represents a base «ViewModel» for shared functionality between the "add" windows for each table.
public abstract partial class AddItemViewModel : ObservableObject
{
    [ObservableProperty]
    private string? _errorMessage;

    [RelayCommand]
    public abstract void Save();

    [RelayCommand]
    public void Cancel()
    {
        Application.Current.Windows
            .OfType<Window>()
            .SingleOrDefault(w => w.DataContext == this)?.Close();
    }
}
