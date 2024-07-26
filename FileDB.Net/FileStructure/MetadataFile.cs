using FileDB.Net.Utils;
using System.Text;
using System.Text.Json;

namespace FileDB.Net.FileStructure
{
    internal class MetadataFile : AFile
    {
        public required string TcpEndPoint { get; set; }
        public required bool IsUsedPassword { get; set; }
        public required Scheme[] Schemes { get; set; }
    }
}