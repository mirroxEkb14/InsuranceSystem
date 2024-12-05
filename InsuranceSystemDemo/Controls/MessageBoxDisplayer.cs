using InsuranceSystemDemo.Utils;
using System.Windows;

namespace InsuranceSystemDemo.Controls;

//
// Summary:
//     Contains a set of methods for displaying different message boxes.
public static class MessageBoxDisplayer
{
    public static void ShowError(string message, string title = "Error") =>
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);

    public static void ShowInfo(string message, string title = "Information") =>
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);

    public static MessageBoxResult ShowDataGridCellEditingSaveChanges() =>
        MessageBox.Show(MessageContainer.DataGridCellEditingSaveChanges,
            "Save Changes",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);
}
