using FileDB.Net.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FileDB.Net.FileStructure
{
    /// <summary>
    /// Abstract file class
    /// </summary>
    internal abstract class AFile
    {
        /// <summary>
        /// Save to file
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="path"> Save to path </param>
        /// <param name="password"> Hashed password form table </param>
        /// <param name="IsHidden"> The file is hidden </param>
        public void Save<T>(string path, byte[]? password, bool IsHidden)
            where T : AFile
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            string json = JsonSerializer.Serialize(this as T);
            byte[] buffer = Encoding.UTF8.GetBytes(json);

            if (password != null)
            {
                buffer = AES.Encrypt(buffer, password);
            }

            using (BinaryWriter sw = new BinaryWriter(new FileStream(path, FileMode.OpenOrCreate)))
            {
                sw.Write(buffer);
            }

            if (IsHidden == true)
            {
                FileInfo info = new FileInfo(path);
                info.Attributes |= FileAttributes.Hidden;
            }
        }

        /// <summary>
        /// Load from file
        /// </summary>
        /// <typeparam name="T"> Data type </typeparam>
        /// <param name="path"> Load form path </param>
        /// <param name="password"> Hashed password form table </param>
        /// <returns> Loaded file's object form </returns>
        public static T? Load<T>(string path, byte[]? password)
            where T : AFile
        {
            using (BinaryReader sr = new BinaryReader(new FileStream(path, FileMode.Open)))
            {
                byte[] buffer = new byte[sr.BaseStream.Length];
                sr.Read(buffer, 0, buffer.Length);

                if (password != null)
                {
                    buffer = AES.Decrypt(buffer, password);
                }

                string json = Encoding.UTF8.GetString(buffer);

                return JsonSerializer.Deserialize<T>(json);
            }
        }
    }
}
