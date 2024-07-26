namespace FileDB.Net
{
    public class Manager
    {
        public string SystemPath { get; private set; }

        public Manager(string path)
        {
            SystemPath = path;

            if (Directory.Exists(path) == false)
            {
                throw new DirectoryNotFoundException(path);
            }

            if (File.Exists(Path.Combine(path, Metadata.Filename)) == false)
            {
                throw new FileNotFoundException(Path.Combine(path, Metadata.Filename));   
            }
        }
    }
}
