using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDB.Net.Utils
{
    /// <summary>
    /// Excepted when directory is existed already
    /// </summary>
    internal class DirectoryFoundException : Exception
    {
        public DirectoryFoundException(string message) : base(message) { }
    }

    /// <summary>
    /// Excepted when file is existed already
    /// </summary>
    internal class FileFoundException : Exception
    {
        public FileFoundException(string message) : base(message) { }
    }

    /// <summary>
    /// Excepted when table is used password but, you do not enter the password
    /// </summary>
    internal class NeedPasswordException : Exception
    {
        public NeedPasswordException(string message) : base(message) { }
    }

    /// <summary>
    /// Excepted when occur duplicated primery key
    /// </summary>
    internal class PriomeryKeyException : Exception
    {
        public PriomeryKeyException(string message) : base(message) { }
    }
}
