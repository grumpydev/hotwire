using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using Hotwire.Shared.Interfaces;

namespace Hotwire.Shared.Services
{
    public class EncryptionService : IEncryptionService
    {
        // Encryption setup
        private string passPhrase = "8e6uga?&t=a!ubayUd6e";
        private string saltValue = "Pa62T4US_m-C@e9#GeCe";
        private string hashAlgorithm = "SHA1";
        private int passwordIterations = 2;
        private string initVector = "h3ku*A_uv!Q-zu9u";
        private int keySize = 256;

        private ISystemInformationService _systemInformation;

        /// <summary>
        /// Initializes a new instance of the EncryptionService class.
        /// </summary>
        /// <param name="systemInformation"></param>
        public EncryptionService(ISystemInformationService systemInformation)
        {
            _systemInformation = systemInformation;

            passPhrase = _systemInformation.DriveSerial;
            saltValue = _systemInformation.CpuId;
        }

        public string Encrypt(string plainText)
        {
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);

            byte[] keyBytes = password.GetBytes(keySize / 8);

            using (RijndaelManaged symmetricKey = new RijndaelManaged())
            {
                symmetricKey.Mode = CipherMode.CBC;
                ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                        cryptoStream.FlushFinalBlock();
                        byte[] cipherTextBytes = memoryStream.ToArray();
                        memoryStream.Close();
                        cryptoStream.Close();
                        return Convert.ToBase64String(cipherTextBytes);
                    }
                }
            }
        }

        public string Decrypt(string encText)
        {
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes = Encoding.ASCII.GetBytes(saltValue);
            byte[] cipherTextBytes = Convert.FromBase64String(encText);

            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);
            byte[] keyBytes = password.GetBytes(keySize / 8);

            string plainText;

            using (RijndaelManaged symmetricKey = new RijndaelManaged())
            {
                symmetricKey.Mode = CipherMode.CBC;
                ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
                using (MemoryStream memoryStream = new MemoryStream(cipherTextBytes))
                {
                    try
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            byte[] plainTextBytes = new byte[cipherTextBytes.Length];
                            int decryptedByteCount;
                            decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                            memoryStream.Close();
                            cryptoStream.Close();
                            plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                        }
                    }
                    catch (CryptographicException)
                    {
                        plainText = string.Empty;
                    }
                }
            }

            return plainText;
        }
    }
}

