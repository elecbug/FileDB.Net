using System.Collections;
using System.Data;
using System.Net;
using System.Numerics;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using FileDB.Net.FileStructure;
using FileDB.Net.Utils;

namespace FileDB.Net
{
    /// <summary>
    /// The class that help you create a set of C# data classes in a file and use them in table form
    /// </summary>
    /// <typeparam name="T"> The class type representing the form of the data </typeparam>
    public class Table<T>
    {
        /// <summary>
        /// File DB metadata from metadata file
        /// </summary>
        private MetadataFile Metadata { get; set; }

        /// <summary>
        /// File DB data(row)'s list, The inner list represents each partition
        /// </summary>
        private List<List<T>> ValuesList { get; set; }
        private byte[]? HashedPassword { get; set; }

        /// <summary>
        /// File DB directory path
        /// </summary>
        public string DBPath { get; private set; }

        /// <summary>
        /// Want to auto save when call insert or remove data
        /// </summary>
        public bool AutoSave { get; set; } = false;

        /// <summary>
        /// Make table object, Is called only Load or Create method
        /// </summary>
        /// <param name="path"> File DB path </param>
        /// <param name="password"> Hashed password for File DB, and unused password if value is null </param>
        private Table(string path, byte[]? password)
        {
            if (Directory.Exists(path) == false)
            {
                throw new DirectoryNotFoundException(path);
            }

            if (File.Exists(Path.Combine(path, Meta.MetadataFileName)) == false)
            {
                throw new FileNotFoundException(Path.Combine(path, Meta.MetadataFileName));
            }

            ValuesList = new List<List<T>>();
            DBPath = path;
            Metadata = MetadataFile.Load<MetadataFile>(Path.Combine(path, Meta.MetadataFileName), password)!;
            HashedPassword = password;

            if (Metadata.IsUsedPassword == true && password == null)
            {
                throw new NeedPasswordException("password is null");
            }

            DirectoryInfo info = new DirectoryInfo(path);
            FileInfo[] dataFiles = info.GetFiles("*" + Meta.DatasetFileExtension);

            foreach (FileInfo file in dataFiles)
            {
                ValuesList.Add(DataSetFile<T>.Load<DataSetFile<T>>(Path.Combine(path, file.Name), password)!.Values);
            }
        }

        /// <summary>
        /// Make table object, Is called only Load or Create method
        /// </summary>
        /// <param name="path"> File DB path </param>
        /// <param name="password"> Password of File DB, and unused password if value is null </param>
        /// <returns> Table object originating from File DB </returns>
        public static Table<T> Load(string path, string? password = null)
        {
            return new Table<T>(path, password != null ? SHA512.HashData(Encoding.UTF8.GetBytes(password)) : null);
        }

        /// <summary>
        /// Make table object, Is called only Load or Create method
        /// </summary>
        /// <param name="path"> File DB path to generate </param>
        /// <param name="prioKey"> Priomery key will use this table </param>
        /// <param name="partitionSize"> Count of one *.data-partition's data </param>
        /// <param name="password"> Password of File DB, and unused password if value is null </param>
        /// <returns> Table object originating from generated </returns>
        public static Table<T> Create(string path, string prioKey, int partitionSize, string? password = null)
        {
            if (Directory.Exists(path) == true && new DirectoryInfo(path).IsEmpty() == false)
            {
                throw new FileFoundException(path);
            }

            Directory.CreateDirectory(path);

            byte[]? hashed = password != null ? SHA512.HashData(Encoding.UTF8.GetBytes(password)) : null;

            MetadataFile metadata = new MetadataFile()
            {
                IsUsedPassword = password != null,
                PrioKey = prioKey,
                PartitionSize = partitionSize,
                //TcpEndPoint = tcpEndPoint.ToString(),
            };
            metadata.Save<MetadataFile>(Path.Combine(path, Meta.MetadataFileName), hashed, true);

            DataSetFile<T> data = new DataSetFile<T>()
            {
                Values = new List<T>() { },
            };
            data.Save<DataSetFile<T>>(Path.Combine(path, "0" + Meta.DatasetFileExtension), hashed, false);

            Table<T> table = new Table<T>(path, hashed);
            return table;
        }

        /// <summary>
        /// Insert data to table
        /// </summary>
        /// <param name="value"> data to insert </param>
        public void Insert(T value)
        {
            object pk = typeof(T).GetProperty(Metadata.PrioKey)!.GetValue(value)!;
            List<T> found = FindAll(x => typeof(T).GetProperty(Metadata.PrioKey)!.GetValue(x)!.Equals(pk));

            if (found.Count != 0)
            {
                throw new PriomeryKeyException(pk.ToString()!);
            }

            foreach (var list in ValuesList)
            {
                if (list.Count < Metadata.PartitionSize)
                {
                    list.Add(value);
                    
                    if (AutoSave == true)
                    {
                        SaveChanges();
                    }

                    return;
                }
            }

            ValuesList.Add(new List<T> { value });

            if (AutoSave == true)
            {
                SaveChanges();
            }
        }

        /// <summary>
        /// Find all data form table
        /// </summary>
        /// <param name="target"> Condition for data to target </param>
        /// <returns> Found all data </returns>
        public List<T> FindAll(Predicate<T> target)
        {
            List<T> result = new List<T>();

            ParallelLoopResult p = Parallel.ForEach(ValuesList, list =>
            {
                List<T> dynamics = list.FindAll(target);

                lock (result)
                {
                    result.AddRange(dynamics);
                }
            });

            while (p.IsCompleted == false) ;
            return result;
        }

        /// <summary>
        /// Remove all data from table
        /// </summary>
        /// <param name="target"> Condition for data to target </param>
        public void RemoveAll(Predicate<T> target)
        {
            ParallelLoopResult p = Parallel.ForEach(ValuesList, list => list.RemoveAll(target));

            while (p.IsCompleted == false) ;

            if (AutoSave == true)
            {
                SaveChanges();
            }
        }

        /// <summary>
        /// Save all changes when before insert or remove data
        /// </summary>
        public void SaveChanges()
        {
            ParallelLoopResult p = Parallel.For(0, ValuesList.Count, i =>
            {
                DataSetFile<T> data = new DataSetFile<T>()
                {
                    Values = ValuesList[i],
                };

                data.Save<DataSetFile<T>>(Path.Combine(DBPath, i + Meta.DatasetFileExtension), HashedPassword, false);
            });

            while (p.IsCompleted == false) ;
        }
    }
}