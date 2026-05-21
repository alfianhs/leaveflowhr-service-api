using System.Security.Cryptography;

namespace LeaveFlowHR.Api.Common.Utilities;

public static class RandomGeneratorUtility
{
    private const string DefaultChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    public static string GenerateRandomString(int length)
    {
        return GenerateRandomString(length, DefaultChars);
    }

    public static string GenerateRandomString(int length, string allowedChars)
    {
        if (length <= 0)
            throw new ArgumentException("Length must be greater than zero.");

        if (string.IsNullOrEmpty(allowedChars))
            throw new ArgumentException("Allowed chars cannot be empty.");

        var result = new char[length];
        var buffer = new byte[length];

        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(buffer);

        for (int i = 0; i < length; i++)
        {
            result[i] = allowedChars[buffer[i] % allowedChars.Length];
        }

        return new string(result);
    }
}