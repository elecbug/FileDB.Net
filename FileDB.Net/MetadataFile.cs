using System.Text.Json;

namespace FileDB.Net
{
    public class MetadataFile
    {
        public required string TcpEndPoint { get; set; }
        public required bool IsUsedPassword { get; set; }
        public required Scheme[] Schemes { get; set; }

        public void Save(string path)
        {
            string json = JsonSerializer.Serialize(this);

            using (StreamWriter sw = new StreamWriter(json))
            {
                sw.Write(json);
            }
        }

        public static MetadataFile? Load(string path)
        {
            using (StreamReader sr = new StreamReader(path))
            {
                string json = sr.ReadToEnd();

                return JsonSerializer.Deserialize<MetadataFile>(json);
            }
        }
    }
}