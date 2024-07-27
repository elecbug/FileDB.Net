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
    public class Manager
    {
        public string DBPath { get; private set; }
        public bool AutoSave { get; set; }
        private MetadataFile Metadata { get; set; }
        private List<dynamic> Values { get; set; }
        private byte[]? HashedPassword { get; set; }

        private Manager(string path, byte[]? password)
        {
            if (Directory.Exists(path) == false)
            {
                throw new DirectoryNotFoundException(path);
            }

            if (File.Exists(Path.Combine(path, Meta.MetadataFileName)) == false)
            {
                throw new FileNotFoundException(Path.Combine(path, Meta.MetadataFileName));
            }

            DBPath = path;
            Metadata = MetadataFile.Load<MetadataFile>(Path.Combine(path, Meta.MetadataFileName), password)!;
            Values = DataSetFile.Load<DataSetFile>(Path.Combine(path, Meta.DatasetFileName), password)!.Values;
            HashedPassword = password;
        }

        public static Manager Load(string path, string? password = null)
        {
            return new Manager(path, password != null ? SHA512.HashData(Encoding.UTF8.GetBytes(password)) : null);
        }

        public static Manager Create(string path, Scheme[] schemes, /*IPEndPoint tcpEndPoint,*/ string? password = null)
        {
            if (Directory.Exists(path) == true && new DirectoryInfo(path).IsEmpty() == false)
            {
                throw new FileFoundException(path);
            }

            if (Scheme.HasDuplicated(schemes) == true)
            {
                throw new DuplicateNameException(JsonSerializer.Serialize(schemes));
            }

            Directory.CreateDirectory(path);

            byte[]? hashed = password != null ? SHA512.HashData(Encoding.UTF8.GetBytes(password)) : null;

            MetadataFile metadata = new MetadataFile()
            {
                IsUsedPassword = password != null,
                Schemes = schemes,
                //TcpEndPoint = tcpEndPoint.ToString(),
            };
            metadata.Save<MetadataFile>(Path.Combine(path, Meta.MetadataFileName), hashed, true);

            DataSetFile data = new DataSetFile()
            {
                Values = new List<dynamic>() { },
            };
            data.Save<DataSetFile>(Path.Combine(path, Meta.DatasetFileName), hashed, false);

            Manager manager = new Manager(path, hashed);
            return manager;
        }

        public void Insert(dynamic value)
        {
            CheckScheme(value);

            Values.Add(value);

            if (AutoSave == true)
            {
                SaveChanges();
            }
        }

        public List<dynamic> FindAll(dynamic value, Predicate<dynamic> target)
        {
            CheckScheme(value);

            return Values.FindAll(target);
        }

        public void RemoveAll(Predicate<dynamic> target)
        {
            Values.RemoveAll(target);

            if (AutoSave == true)
            {
                SaveChanges();
            }
        }

        public void SaveChanges()
        {
            DataSetFile data = new DataSetFile()
            {
                Values = Values
            };

            data.Save<DataSetFile>(Path.Combine(DBPath, Meta.DatasetFileName), HashedPassword, false);
        }

        private void CheckScheme(dynamic value)
        {
            Scheme[] schemes = Metadata.Schemes;
            Type typeOfValue = value.GetType();

            foreach (Scheme scheme in schemes)
            {
                if (typeOfValue.GetProperty(scheme.Field) == null)
                {
                    throw new SchemeMismatchException(scheme.Field);
                }

                object? inValue = typeOfValue.GetProperty(scheme.Field)!.GetValue(value);

                if ((inValue is INumber<int> && scheme.Type.HasFlag(SchemeType.CanInt) == false) ||
                    (inValue is INumber<float> && scheme.Type.HasFlag(SchemeType.CanFloat) == false) ||
                    (inValue is null && scheme.Type.HasFlag(SchemeType.Nullable) == false) ||
                    ((inValue is string || inValue is char) && scheme.Type.HasFlag(SchemeType.CanString) == false) ||
                    (inValue is IList && scheme.Type.HasFlag(SchemeType.CanList) == false) ||
                    (inValue is bool && scheme.Type.HasFlag(SchemeType.CanBool) == false))
                {
                    throw new SchemeMismatchException(scheme.Field + ": " + inValue);
                }

                if (inValue is IList && scheme.Type.HasFlag(SchemeType.CanList) == true)
                {
                    foreach (var item in (inValue as IList)!)
                    {
                        if ((item is INumber<int> && scheme.Type.HasFlag(SchemeType.CanInt) == false) ||
                            (item is INumber<float> && scheme.Type.HasFlag(SchemeType.CanFloat) == false) ||
                            (item is null && scheme.Type.HasFlag(SchemeType.Nullable) == false) ||
                            ((item is string || inValue is char) && scheme.Type.HasFlag(SchemeType.CanString) == false) ||
                            (item is IList && scheme.Type.HasFlag(SchemeType.CanMultipleList) == false) ||
                            (item is bool && scheme.Type.HasFlag(SchemeType.CanBool) == false))
                        {
                            throw new SchemeMismatchException(scheme.Field + ": " + inValue + "." + item);
                        }
                    }
                }
            }
        }
    }
}