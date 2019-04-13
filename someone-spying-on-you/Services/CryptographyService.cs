using System;
using System.Security.Cryptography;
using System.Text;

namespace SomeOneSpyingOnYou.Services
{
    public class CryptographyService : IDisposable
    {
        private readonly RSACryptoServiceProvider rsaProvider;
        private readonly UnicodeEncoding byteConverter;
        public CryptographyService()
        {
            rsaProvider = new RSACryptoServiceProvider(2048);
            rsaProvider.ExportParameters(false);
            byteConverter = new UnicodeEncoding();
        }

        public void Dispose()
        {
        }

        public string Encrypt(string text)
        {
            var encryptedData = RSAEncrypt(byteConverter.GetBytes(text), rsaProvider.ExportParameters(false), false);
            return Convert.ToBase64String(encryptedData);
        }

        public string Decrypt(string encryptedBytesString)
        {            
            var decryptedData = RSADecrypt(Convert.FromBase64String(encryptedBytesString), rsaProvider.ExportParameters(true), false);
            return byteConverter.GetString(decryptedData);
        }




        public byte[] RSAEncrypt(byte[] DataToEncrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {
            try
            {
                byte[] encryptedData;

                //Import the RSA Key information. This only needs
                //toinclude the public key information.
                rsaProvider.ImportParameters(RSAKeyInfo);

                //Encrypt the passed byte array and specify OAEP padding.  
                //OAEP padding is only available on Microsoft Windows XP or
                //later.  
                encryptedData = rsaProvider.Encrypt(DataToEncrypt, DoOAEPPadding);
                return encryptedData;
            }
            //Catch and display a CryptographicException  
            //to the console.
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);

                return null;
            }

        }

        public byte[] RSADecrypt(byte[] DataToDecrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {
            try
            {
                byte[] decryptedData;
                //Create a new instance of RSACryptoServiceProvider.
                //Import the RSA Key information. This needs
                //to include the private key information.
                rsaProvider.ImportParameters(RSAKeyInfo);

                //Decrypt the passed byte array and specify OAEP padding.  
                //OAEP padding is only available on Microsoft Windows XP or
                //later.  
                decryptedData = rsaProvider.Decrypt(DataToDecrypt, DoOAEPPadding);

                return decryptedData;
            }
            //Catch and display a CryptographicException  
            //to the console.
            catch (CryptographicException e)
            {
                Console.WriteLine(e.ToString());

                return null;
            }

        }
    }
}
