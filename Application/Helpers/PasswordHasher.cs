using System.Security.Cryptography;

namespace Application.Helpers;

public static class PasswordHasher
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100_000;

    public static (byte[] Hash, byte[] Salt) HashPassword(string password)
    {
        ArgumentException.ThrowIfNullOrEmpty(password);
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize);
        return (hash, salt);
    }

    public static bool VerifyPassword(string password, byte[]? hash, byte[]? salt)
    {
        ArgumentException.ThrowIfNullOrEmpty(password);
        if (hash is not { Length: HashSize } || salt is not { Length: SaltSize })
            return false;

        var computed = Rfc2898DeriveBytes.Pbkdf2(
            password,
            salt,
            Iterations,
            HashAlgorithmName.SHA256,
            HashSize);
        return CryptographicOperations.FixedTimeEquals(computed, hash);
    }
}
