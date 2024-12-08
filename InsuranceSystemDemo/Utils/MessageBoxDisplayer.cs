using System.Windows;

namespace InsuranceSystemDemo.Utils;

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

    public static MessageBoxResult ShowClientDeletionConfirmation(string firstName, string lastName) =>
        MessageBox.Show(MessageContainer.GetDeleteClientConfirmation(firstName, lastName),
            "Delete Client",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

    public static MessageBoxResult ShowAddressDeletionConfirmation(string street, string city) =>
        MessageBox.Show(MessageContainer.GetDeleteAddressConfirmation(street, city),
            "Delete Address",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

    public static MessageBoxResult ShowBranchDeletionConfirmation(string branchName, string phone) =>
    MessageBox.Show(
        MessageContainer.GetDeleteBranchConfirmation(branchName, phone),
        "Delete Branch",
        MessageBoxButton.YesNo,
        MessageBoxImage.Question
    );
    public static MessageBoxResult ShowInsuranceTypeDeletionConfirmation(string description)
    {
        return MessageBox.Show(
            $"Are you sure you want to delete the insurance type \"{description}\"?",
            "Confirm Deletion",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);
    }

    public static MessageBoxResult ShowContractDeletionConfirmation(string contractId)
    {
        return MessageBox.Show($"Are you sure you want to delete contract with ID {contractId}?",
                               "Delete Confirmation",
                               MessageBoxButton.YesNo,
                               MessageBoxImage.Warning);
    }
}
