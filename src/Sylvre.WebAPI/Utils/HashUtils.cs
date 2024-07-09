using System;
using System.Text;
using System.Security.Cryptography;

namespace Sylvre.WebAPI.Utils
{
    /// <summary>
    /// Provides functionality for hashing.
    /// </summary>
    public static class HashUtils
    {
        /// <summary>
        /// Generates a hash and salt from a given string.
        /// </summary>
        /// <param name="str">The string to hash and salt.</param>
        /// <param name="hash">The out variable to set hash to.</param>
        /// <param name="salt">The out variable to set the salt to.</param>
        public static void GenerateHashAndSaltFromString(string str,
            out byte[] hash, out byte[] salt)
        {
            using (var hmac = new HMACSHA512())
            {
                salt = hmac.Key;
                hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(str));
            }
        }

        /// <summary>
        /// Checks if a string is valid against a given hash and salt.
        /// </summary>
        /// <param name="str">The string to check.</param>
        /// <param name="hashToCheck">The hash to check against.</param>
        /// <param name="salt">The salt to use.</param>
        /// <returns>True if valid, false otherwise.</returns>
        public static bool IsStringValidAgainstHashAndSalt(string str,
            byte[] hashToCheck, byte[] salt)
        {
            using (var hmac = new HMACSHA512(salt))
            {
                var hmacComputedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(str));
                for (int i = 0; i < hmacComputedHash.Length; i++)
                {
                    if (hmacComputedHash[i] != hashToCheck[i])
                        return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Generates a consistent hash from a given string.
        /// </summary>
        /// <param name="str">The string to hash.</param>
        /// <returns>The hashed value.</returns>
        public static string GenerateConsistentHashFromString(string str)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(str));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
