using System.Text.Json;

namespace FileDB.Net.FileStructure
{
    /// <summary>
    /// Data set file form of class
    /// </summary>
    /// <typeparam name="T"> Data type </typeparam>
    internal class DataSetFile<T> : AFile
    {
        /// <summary>
        /// Data(rows) of one data set file
        /// </summary>
        public required List<T> Values { get; set; }
    }
}