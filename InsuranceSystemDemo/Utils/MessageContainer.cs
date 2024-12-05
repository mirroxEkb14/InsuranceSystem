namespace InsuranceSystemDemo.Utils;

//
// Summary:
//     Contains a set of messages that can be displayed in message boxes or
//         used as default values for text boxes on the UI side.
public static class MessageContainer
{
    private const string UnexpectedErrorMessage = "An unexpected error occurred: {0}";

    public const string AdminRole = "Admin";
    public const string UserRole = "User";

    public const string KlientiTableName = "Klienti";
    public const string PoliciesTableName = "ActivePolicies";

    public const string AddFunctionalityNotSupported = "Add functionality is not supported for this table.";

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

    public static string GetUnexpectedErrorMessage(string errorMessage = "no message available") =>
        string.Format(UnexpectedErrorMessage, errorMessage);
}
