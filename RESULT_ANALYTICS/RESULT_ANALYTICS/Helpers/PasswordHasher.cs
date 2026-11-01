
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace RESULT_ANALYTICS.Helpers
{
    internal class PasswordHasher
    {
        private const int Iterations = 600000;
        private const int SaltSize = 16;
        private const int HashSize = 32;

        // Ham bam mat khau PBKDF2-SHA256
        public static string HashPassword(string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException(nameof(password));
            }

            byte[] salt = new byte[SaltSize];

            using (RandomNumberGenerator rng =
                RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            using (Rfc2898DeriveBytes pbkdf2 =
                new Rfc2898DeriveBytes(
                    password,
                    salt,
                    Iterations,
                    HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(HashSize);

                return "PBKDF2-SHA256$" +
                       Iterations + "$" +
                       Convert.ToBase64String(salt) + "$" +
                       Convert.ToBase64String(hash);
            }
        }

        // Ham kiem tra mat khau dang nhap
        public static bool VerifyPassword(
            string password,
            string storedHash)
        {
            if (password == null ||
                string.IsNullOrWhiteSpace(storedHash))
            {
                return false;
            }

            string[] parts = storedHash.Split('$');

            if (parts.Length != 4 ||
                parts[0] != "PBKDF2-SHA256")
            {
                return false;
            }

            int iterations;

            if (!int.TryParse(parts[1], out iterations) ||
                iterations < 100000 ||
                iterations > 2000000)
            {
                return false;
            }

            try
            {
                byte[] salt =
                    Convert.FromBase64String(parts[2]);

                byte[] expectedHash =
                    Convert.FromBase64String(parts[3]);

                if (salt.Length < SaltSize ||
                    expectedHash.Length != HashSize)
                {
                    return false;
                }

                byte[] actualHash;

                using (Rfc2898DeriveBytes pbkdf2 =
                    new Rfc2898DeriveBytes(
                        password,
                        salt,
                        iterations,
                        HashAlgorithmName.SHA256))
                {
                    actualHash =
                        pbkdf2.GetBytes(expectedHash.Length);
                }

                int difference = 0;

                for (int i = 0; i < expectedHash.Length; i++)
                {
                    difference |=
                        expectedHash[i] ^ actualHash[i];
                }

                return difference == 0;
            }
            catch (FormatException)
            {
                return false;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
    }
}
