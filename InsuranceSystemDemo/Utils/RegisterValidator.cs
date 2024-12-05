using System.Text.RegularExpressions;

namespace InsuranceSystemDemo.Utils;

//
// Summary:
//     Contains a set of methods for validation user's input when registering.
public static class RegisterValidator
{
    public static string? ValidateUsername(string? username)
    {
        if (string.IsNullOrEmpty(username))
            return MessageContainer.RegisterRequiredUsername;
        return null;
    }

    public static string? ValidateFirstName(string? firstName)
    {
        if (string.IsNullOrEmpty(firstName))
            return MessageContainer.RegisterRequiredFirstName;
        return null;
    }

    public static string? ValidateLastName(string? lastName)
    {
        if (string.IsNullOrEmpty(lastName))
            return MessageContainer.RegisterRequiredLastName;
        return null;
    }

    public static string? ValidateEmail(string? email)
    {
        if (string.IsNullOrEmpty(email))
            return MessageContainer.RegisterRequiredEmail;
        if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            return MessageContainer.RegisterInvalidEmail;
        return null;
    }

    public static string? ValidatePhone(string? phone)
    {
        if (string.IsNullOrEmpty(phone))
            return MessageContainer.RegisterRequiredPhone;
        if (!Regex.IsMatch(phone, @"^\+?\d{10,15}$"))
            return MessageContainer.RegisterInvalidPhone;
        return null;
    }

    public static string? ValidatePassword(string? password, string? confirmPassword)
    {
        if (string.IsNullOrEmpty(password))
            return MessageContainer.RegisterRequiredPassword;
        if (password != confirmPassword)
            return MessageContainer.RegisterInvalidPassword;
        return null;
    }
}
