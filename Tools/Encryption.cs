using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    public class Encryption
    {
        // Obfuscate a file name using integer obfuscation
        public static string ObfuscateFileName(string fileName)
        {
            string obfuscated = "";
            foreach (char c in fileName)
            {
                int asciiValue = (int)c;
                int obfuscatedValue = asciiValue * 7 + 3; // multiply by 7 and add 3
                obfuscated += obfuscatedValue.ToString() + "_";
            }
            obfuscated = obfuscated.Substring(0, obfuscated.Length - 1);
            return obfuscated.Trim();
        }   

        // Deobfuscate an obfuscated file name using integer obfuscation
        public static string DeobfuscateFileName(string obfuscatedFileName)
        {
            string[] obfuscatedValues = obfuscatedFileName.Split('_');
            string decrypted = "";
            foreach (string obfuscatedValue in obfuscatedValues)
            {
                int intValue = int.Parse(obfuscatedValue);
                int decryptedValue = (intValue - 3) / 7; // subtract 3 and divide by 7
                decrypted += (char)decryptedValue;
            }
            return decrypted;
        }
        
        private static readonly byte[] Salt = Encoding.ASCII.GetBytes("Your Salt Here");

        public static string Encrypt(string plainText, string password)
        {
            var algorithm = GetAlgorithm(password);

            byte[] plainBytes = Encoding.Unicode.GetBytes(plainText);

            using (ICryptoTransform encryptor = algorithm.CreateEncryptor(algorithm.Key, algorithm.IV))
            using (MemoryStream ms = new MemoryStream())
            using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                cs.Write(plainBytes, 0, plainBytes.Length);
                cs.FlushFinalBlock();

                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public static string Decrypt(string encryptedText, string password)
        {
            var algorithm = GetAlgorithm(password);

            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);

            using (ICryptoTransform decryptor = algorithm.CreateDecryptor(algorithm.Key, algorithm.IV))
            using (MemoryStream ms = new MemoryStream(encryptedBytes))
            using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            {
                byte[] decryptedBytes = new byte[encryptedBytes.Length];
                int decryptedByteCount = cs.Read(decryptedBytes, 0, decryptedBytes.Length);

                return Encoding.Unicode.GetString(decryptedBytes, 0, decryptedByteCount);
            }
        }

        private static RijndaelManaged GetAlgorithm(string password)
        {
            var key = new Rfc2898DeriveBytes(password, Salt);

            var algorithm = new RijndaelManaged();
            algorithm.Key = key.GetBytes(algorithm.KeySize / 8);
            algorithm.IV = key.GetBytes(algorithm.BlockSize / 8);

            return algorithm;
        }
    }
}
