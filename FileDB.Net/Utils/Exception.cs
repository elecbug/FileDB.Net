using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDB.Net.Utils
{
    internal class DirectoryFoundException : Exception
    {
        public DirectoryFoundException(string message) : base(message) { }
    }

    internal class FileFoundException : Exception
    {
        public FileFoundException(string message) : base(message) { }
    }

    internal class SchemeMismatchException : Exception
    {
        public SchemeMismatchException(string message) : base(message) { }
    }
}
