using System;
using System.Security.Cryptography;
using System.Text;

namespace Boxsie.Network.Core
{
    public static class CryptoHelper
    {
        private static readonly RNGCryptoServiceProvider Crypto;
       
        static CryptoHelper()
        {
            Crypto = new RNGCryptoServiceProvider();
        }

        public static string GetRandomSalt()
        {
            var saltBytes = new byte[32];
            Crypto.GetNonZeroBytes(saltBytes);
            return Convert.ToBase64String(saltBytes);
        }

        public static string ToBase64String(this string text)
        {
            var bytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(bytes);
        }

        public static string GetEncryptedHash(string text, string salt)
        {
            var sha = new SHA256CryptoServiceProvider();
            var dataBytes = Encoding.UTF8.GetBytes(string.Concat(text, salt));
            var resultBytes = sha.ComputeHash(dataBytes);
            return Convert.ToBase64String(resultBytes);
        }
    }
}
