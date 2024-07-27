using FileDB.Net.Utils;
using System.Text;
using System.Text.Json;

namespace FileDB.Net.FileStructure
{
    /// <summary>
    /// Metadata file's form of class
    /// </summary>
    internal class MetadataFile : AFile
    {
        /// <summary>
        /// Information of table is used password
        /// </summary>
        public required bool IsUsedPassword { get; set; }

        /// <summary>
        /// Priomery key name of table
        /// </summary>
        public required string PrioKey { get; set; }

        /// <summary>
        /// Count of one *.data-partition's data
        /// </summary>
        public required int PartitionSize { get; set; }
    }
}