using System.Security.Cryptography;
using System.Text;

namespace InsuranceSystemDemo.Utils;

//
// Summary:
//     Provides a set of basic operations for hashing and verifying users' passwords.
public static class PasswordHasher
{
    public static string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty.");

        var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    public static bool VerifyPassword(string? enteredPassword, string? storedHash)
    {
        if (string.IsNullOrWhiteSpace(enteredPassword) || string.IsNullOrWhiteSpace(storedHash))
            return false;

        var enteredHash = HashPassword(enteredPassword);
        return enteredHash == storedHash;
    }
}
