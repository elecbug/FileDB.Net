using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDB.Net.Utils
{
    /// <summary>
    /// Static Extension methods
    /// </summary>
    internal static class Extension
    {
        /// <summary>
        /// Check empty directory(non-file and non-sub-dirctory)
        /// </summary>
        /// <param name="info"> Directoryinfo </param>
        /// <returns> This directory have not anything </returns>
        public static bool IsEmpty(this DirectoryInfo info)
        {
            return info.GetFiles().Length == 0 && info.GetDirectories().Length == 0;
        }
    }
}
