using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FileDB.Net.Utils
{
    /// <summary>
    /// Custom AES
    /// </summary>
    internal static class AES
    {
        /// <summary>
        /// Inner AES
        /// </summary>
        private static Aes Aes { get; set; } = Aes.Create();

        /// <summary>
        /// Encrypt data using AES
        /// </summary>
        /// <param name="data"> Data to encrypt </param>
        /// <param name="hashedKey"> Key as hashed password </param>
        /// <returns> Encrypted data </returns>
        public static byte[] Encrypt(byte[] data, byte[] hashedKey)
        {
            Aes.KeySize = 256;
            Aes.Key = hashedKey[32..64];

            return Aes.EncryptCbc(data, hashedKey[0..16]);
        }

        /// <summary>
        /// Decrypt data using AES
        /// </summary>
        /// <param name="data"> Data to Decrypt </param>
        /// <param name="hashedKey"> Key as hashed password </param>
        /// <returns> Decrypted data </returns>
        public static byte[] Decrypt(byte[] data, byte[] hashedKey)
        {
            Aes.KeySize = 256;
            Aes.Key = hashedKey[32..64];

            return Aes.DecryptCbc(data, hashedKey[0..16]);
        }
    }
}
