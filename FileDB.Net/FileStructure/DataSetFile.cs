using System.Text.Json;

namespace FileDB.Net.FileStructure
{
    internal class DataSetFile<T> : AFile
    {
        public required List<T> Values { get; set; }
    }
}