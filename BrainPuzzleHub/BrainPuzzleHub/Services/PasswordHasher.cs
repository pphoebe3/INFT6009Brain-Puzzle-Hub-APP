using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using System.Text;



namespace BrainPuzzleHub.Services;


//Static utility class:
//responsible for salting and hashing passwords, never storing passwords in plaintext.
public static class PasswordHasher
{
    //Generate random salt values
    public static string GenerateSalt()
    {
        var bytes = RandomNumberGenerator.GetBytes(16);
        return Convert.ToBase64String(bytes);
    }

    //Use salt to perform a SHA256 hash on the password.
    public static string Hash(string password, string salt)
    {
        using var sha256 = SHA256.Create();
        var combined = Encoding.UTF8.GetBytes(password + salt);
        var hashBytes = sha256.ComputeHash(combined);
        return Convert.ToBase64String(hashBytes);
    }

    //Verify password match
    public static bool Verify(string password, string salt, string storedHash)
    {
        var computed = Hash(password, salt);
        return computed == storedHash;
    }
}
