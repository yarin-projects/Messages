using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using SecureMessagesAPI.Interfaces;

namespace SecureMessagesAPI.Encryption
{
    public class EncryptionHelper : IEncryptionHelper
    {
        private const string KeyFilePath = "Encryption/key.json";
        private const int AesKeySize = 256; // 256 bits
        private const int AesBlockSize = 128; // 128 bits, which is 16 bytes

        // Method to generate a new encryption key
        private static byte[] GenerateEncryptionKey()
        {
            using (var aes = Aes.Create())
            {
                aes.KeySize = AesKeySize;
                aes.GenerateKey();
                return aes.Key;
            }
        }

        // Method to save the encryption key to a JSON file
        private static void SaveEncryptionKey(byte[] key)
        {
            string keyJson = JsonConvert.SerializeObject(key);
            File.WriteAllText(KeyFilePath, keyJson);
        }

        // Method to load the encryption key from the JSON file
        private static byte[] LoadEncryptionKey()
        {
            if (File.Exists(KeyFilePath))
            {
                string keyJson = File.ReadAllText(KeyFilePath);
                return JsonConvert.DeserializeObject<byte[]>(keyJson)!;
            }
            else
            {
                // If the key file doesn't exist, generate a new key and save it
                byte[] newKey = GenerateEncryptionKey();
                SaveEncryptionKey(newKey);
                return newKey;
            }
        }

        public string Encrypt(string plainText)
        {
            byte[] key = LoadEncryptionKey();
            byte[] iv;
            byte[] encrypted;

            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.BlockSize = AesBlockSize;
                aes.GenerateIV();
                iv = aes.IV;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (var sw = new StreamWriter(cs))
                        {
                            sw.Write(plainText);
                        }
                        encrypted = ms.ToArray();
                    }
                }
            }

            // Combine IV and encrypted data
            byte[] combinedIvEnc = new byte[iv.Length + encrypted.Length];
            Array.Copy(iv, 0, combinedIvEnc, 0, iv.Length);
            Array.Copy(encrypted, 0, combinedIvEnc, iv.Length, encrypted.Length);

            return Convert.ToBase64String(combinedIvEnc);
        }

        public string Decrypt(string cipherText)
        {
            byte[] combinedIvEnc = Convert.FromBase64String(cipherText);
            byte[] key = LoadEncryptionKey();
            byte[] iv = new byte[AesBlockSize / 8];
            byte[] encrypted = new byte[combinedIvEnc.Length - iv.Length];

            Array.Copy(combinedIvEnc, 0, iv, 0, iv.Length);
            Array.Copy(combinedIvEnc, iv.Length, encrypted, 0, encrypted.Length);

            string plaintext;

            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (var ms = new MemoryStream(encrypted))
                {
                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        using (var sr = new StreamReader(cs))
                        {
                            plaintext = sr.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }
    }
}
