﻿namespace InsuranceSystemDemo.Utils;

//
// Summary:
//     Contains a set of messages that can be displayed in message boxes or
//         used as default values for text boxes on the UI side.
public static class MessageContainer
{
    private const string UnexpectedErrorMessage = "An unexpected error occurred: {0}";
    private const string DeleteClientConfirmation = "Are you sure you want to delete the client {0} {1}?";
    private const string DeleteAddressConfirmation = "Are you sure you want to delete the address at {0} {1}?";
    private const string DeleteBranchConfirmation = "Are you sure you want to delete the branch {0} with phone number {1}?";

    public const string CloningError = "Unsupported type for cloning";

    public const string AdminRole = "Admin";
    public const string UserRole = "User";

    public const string KlientiTableName = "Klienti";
    public const string AdresyTableName = "Adresy";
    public const string PobockyTableName = "Pobocky";
    public const string TypPojistkyTableName = "TypPojistky";
    public const string ContractsTableName = "PojistnaSmlouva";
    public const string ZamestnanecTableName= "Zamestnanec";
    public const string PohledavkaTableName = "Pohledavka";

    public const string AddFunctionalityNotSupported = "Add functionality is not supported for this table.";

    public const string DeleteFunctionalityNotSupported = "Delete functionality is not supported for this table.";
    public const string DeleteItemNotSelected = "Please select an item to delete.";
    public const string DeleteClientSuccess = "Client was successfully deleted.";
    public const string DeleteAddressSuccess = "Address was successfully deleted.";

    public const string ConnectionStringKey = "ConnectionStrings:DefaultConnection";
    public const string LoginUsernameHint = "Enter your username here...";
    public const string LoginUsernameErrorMessage = "Please enter a valid username.";
    public const string LoginPasswordErrorMessage = "Please enter a valid password.";
    public const string LoginInvalidUsernameMessage = "Invalid credentials.";
    public const string LoginInvalidPasswordMessage = "Invalid password.";
    public const string LoginInvalidUserRoleMessage = "Invalid role assigned to this user.";
    public const string RegisterRequiredUsername = "Username is required.";
    public const string RegisterRequiredFirstName = "First name is required.";
    public const string RegisterRequiredLastName = "Last name is required.";
    public const string RegisterRequiredEmail = "Email is required.";
    public const string RegisterInvalidEmail = "Invalid email format.";
    public const string RegisterRequiredPhone = "Phone number is required.";
    public const string RegisterInvalidPhone = "Invalid phone number format.";
    public const string RegisterRequiredPassword = "Password is required.";
    public const string RegisterInvalidPassword = "Passwords do not match.";
    public const string RegisterExistingUsername = "Such a username already exists.";
    public const string RegisterExistingEmail = "Such an email already exists.";
    public const string RegisterExistingPhone = "Such a phone number already exists.";
    public const string RegisterSuccess = "Registration successful!";

    public const string AddClientRequiredFirstName = "First Name is required.";
    public const string AddClientRequiredLastName = "Last Name is required.";
    public const string AddClientRequiredEmail = "Email is required.";
    public const string AddClientInvalidEmail = "Invalid email format.";
    public const string AddClientRequiredPhone = "Phone number is required.";
    public const string AddClientRequiredStreet = "Street is required.";
    public const string AddClientRequiredCity = "City is required.";
    public const string AddClientRequiredCountry = "Country is required.";
    public const string AddClientRequiredHouseNumber = "House Number is required.";
    public const string AddClientRequiredPostalCode = "Postal Code is required.";
    public const string AddClientSuccess = "Client added successfully!";

    public const string DataGridCellEditingSaveChanges = "Do you want to save the changes?";
    public const string DataGridCellEditingChangesSaved = "Changes saved successfully.";

    public const string AddAddressRequiredStreet = "Street is required.";
    public const string AddAddressRequiredCity = "City is required.";
    public const string AddAddressRequiredCountry = "Country is required.";
    public const string AddAddressRequiredHouseNumber = "House Number is required.";
    public const string AddAddressInvalidHouseNumber = "House number must be a valid number.";
    public const string AddAddressRequiredPostalCode = "Postal Code is required.";
    public const string AddAddressInvalidPostalCode = "Postal code must be a valid number.";
    public const string AddAddressSuccess = "Address added successfully!";
    public const string AddAddressFailure = "Failed to add address (retrieving the new address ID).";

    public const string AddBranchRequiredName = "Branch Name is required.";
    public const string AddBranchRequiredPhone = "Phone number is required.";
    public const string AddBranchRequiredAddress = "Address is required.";
    public const string AddBranchSuccess = "Branch added successfully!";

    public const string AddInsuranceTypeRequiredAvailability = "Availability is required.";
    public const string AddInsuranceTypeRequiredConditions = "Conditions are required.";
    public const string AddInsuranceTypeInvalidMaxCoverage = "Maximum coverage must be greater than zero.";
    public const string AddInsuranceTypeInvalidMinCoverage = "Minimum coverage must be greater than zero.";
    public const string AddInsuranceTypeInvalidCoverage = "Minimum coverage cannot exceed maximum coverage.";
    public const string AddInsuranceTypeSuccess = "Insurance type added successfully!";

    public const string AddContractInvalidAmount = "Insurance amount must be greater than 0.";
    public const string AddContractInvalidDates = "End date must be after the start date.";
    public const string AddContractInvalidClient = "Client must be selected.";
    public const string AddContractInvalidBranch = "Branch must be selected.";
    public const string AddContractInvalidPolicyType = "Policy type must be selected.";
    public const string AddContractSuccess = "Contract added successfully!";

    public const string AddEmployeeErrorRetrievingID = "Failed to retrieve the ID of the new Zamestnanec.";
    public const string AddEmployeeInvalidRole = "Role must be selected.";
    public const string AddEmployeeInvalidName = "First Name must be entered.";
    public const string AddEmployeeInvalidSurname = "Last Name must be entered.";
    public const string AddEmployeeInvalidPhone = "Phone number must be entered.";
    public const string AddEmployeeInvalidBranch = "Branch must be selected.";
    public const string AddEmployeeInvalidAddress = "Address must be selected.";

    public static string GetUnexpectedErrorMessage(string errorMessage = "no message available") =>
        string.Format(UnexpectedErrorMessage, errorMessage);

    public static string GetDeleteClientConfirmation(string firstName, string lastName) =>
        string.Format(DeleteClientConfirmation, firstName, lastName);

    public static string GetDeleteAddressConfirmation(string street, string city) =>
        string.Format(DeleteAddressConfirmation, street, city);

    public static string GetDeleteBranchConfirmation(string branchName, string phone) =>
        string.Format(DeleteBranchConfirmation, branchName, phone);
}
