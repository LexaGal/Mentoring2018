using System;
using System.Linq;
using System.Security.Cryptography;

namespace PasswordHasher
{
    class Program
    {
        static void Main(string[] args)
        {
            for (var i = 0; i < 1000; i++)
            {
                GeneratePasswordHashUsingSalt($"qwerty{i}", GetBytesFast(32));
            }
            Console.ReadKey();
        }

        public static string GeneratePasswordHashUsingSalt(string passwordText, byte[] salt)
        {
            var iterate = 10000;
            var pbkdf2 = new Rfc2898DeriveBytes(passwordText, salt, iterate);
            byte[] hash = GetBytes(pbkdf2, 20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            var passwordHash = Convert.ToBase64String(hashBytes);
            return passwordHash;
        }

        private static byte[] GetBytes(Rfc2898DeriveBytes pbkdf2, int size)
        {
            return GetBytesFast(size);
            //return GetBytesSlow(pbkdf2, size);
        }

        private static byte[] GetBytesSlow(Rfc2898DeriveBytes pbkdf2, int size)
        {
            return pbkdf2.GetBytes(size);                        
        }

        private static byte[] GetBytesFast(int maxLength)
        {
            var random = new RNGCryptoServiceProvider();
            byte[] salt = new byte[maxLength];
            random.GetNonZeroBytes(salt);
            return salt;
        }
    }
}
