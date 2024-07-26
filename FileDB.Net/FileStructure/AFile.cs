using FileDB.Net.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FileDB.Net.FileStructure
{
    internal abstract class AFile
    {
        public void Save<T>(string path, byte[]? password)
            where T : AFile
        {
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

            FileInfo info = new FileInfo(path);
            info.Attributes |= FileAttributes.Hidden;
        }

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
