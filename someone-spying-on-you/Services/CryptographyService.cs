using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SomeOneSpyingOnYou.Services
{
    public sealed class CryptographyService : IDisposable
    {
        static private string PasswordHash = ProjectConfigurationManager.GetValue("scr.PasswordHash");
        static private string SaltKey = ProjectConfigurationManager.GetValue("scr.SaltKey");
        static private string VIKey = ProjectConfigurationManager.GetValue("scr.VIKey");

        private static RijndaelManaged symmetricKey = new RijndaelManaged();

        public CryptographyService()
        {
            if (string.IsNullOrEmpty(SaltKey) || string.IsNullOrEmpty(VIKey))
            {
                symmetricKey.GenerateIV();
                VIKey = Convert.ToBase64String(symmetricKey.IV);
                ProjectConfigurationManager.AddOrReplace("scr.VIKey", VIKey);
                symmetricKey.GenerateKey();
                SaltKey = Convert.ToBase64String(symmetricKey.Key);
                ProjectConfigurationManager.AddOrReplace("scr.SaltKey", SaltKey);
            }

            if (string.IsNullOrEmpty(PasswordHash))
            {
                Console.WriteLine("Now, please enter a password.");
                Console.WriteLine("This password will be used for encrypt your information.");
                PasswordHash = Console.ReadLine();
                ProjectConfigurationManager.AddOrReplace("scr.PasswordHash", PasswordHash);
            }
        }

        public void Dispose()
        {
        }

        public string Encrypt(string text)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(text);
            symmetricKey.Mode = CipherMode.CBC;
            symmetricKey.Padding = PaddingMode.Zeros;

            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Convert.FromBase64String(SaltKey)).GetBytes(256 / 8);
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Convert.FromBase64String(VIKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(cipherTextBytes);
        }

        public string Decrypt(string encryptedText)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Convert.FromBase64String(SaltKey)).GetBytes(256 / 8);
            symmetricKey.Mode = CipherMode.CBC;
            symmetricKey.Padding = PaddingMode.None;

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Convert.FromBase64String(VIKey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }


    }
}
