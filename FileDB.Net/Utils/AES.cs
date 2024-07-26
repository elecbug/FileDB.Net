using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FileDB.Net.Utils
{
    internal static class AES
    {
        private static Aes Aes { get; set; } = Aes.Create();

        public static byte[] Encrypt(byte[] data, byte[] hashedKey)
        {
            Aes.KeySize = 256;
            Aes.Key = hashedKey[32..64];

            return Aes.EncryptCbc(data, hashedKey[0..16]);
        }

        public static byte[] Decrypt(byte[] data, byte[] hashedKey)
        {
            Aes.KeySize = 256;
            Aes.Key = hashedKey[32..64];

            return Aes.DecryptCbc(data, hashedKey[0..16]);
        }
    }
}
