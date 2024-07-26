using System.Text.Json;

namespace FileDB.Net.FileStructure
{
    internal class DataSetFile : AFile
    {
        public required List<dynamic> Values { get; set; }
    }
}