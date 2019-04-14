using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SomeOneSpyingOnYou.Services
{
    public sealed class CryptographyService : IDisposable
    {
        static private string PasswordHash = "P@@Sw0rd";
        static private string SaltKey = "ZP5sji8BpzOTcm8lZikg+NbeN87JgzDJ6SEdVwoio4o=";
        static private string VIKey = "Nj/uRmU76g1Pa4PRjO0B0w==";

        private static RijndaelManaged symmetricKey = new RijndaelManaged();

        public CryptographyService()
        {
            //PasswordHash = ProjectConfigurationManager.GetValue("PasswordHash");
            //SaltKey = ProjectConfigurationManager.GetValue("SaltKey");
            //VIKey = ProjectConfigurationManager.GetValue("VIKey");

            //symmetricKey = new RijndaelManaged();
            //symmetricKey.GenerateIV();
            //VIKey = Convert.ToBase64String(symmetricKey.IV);
            //symmetricKey.GenerateKey();
            //SaltKey = Convert.ToBase64String(symmetricKey.Key);

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
