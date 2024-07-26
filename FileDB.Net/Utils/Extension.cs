using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileDB.Net.Utils
{
    internal static class Extension
    {
        public static bool IsEmpty(this DirectoryInfo info)
        {
            return info.GetFiles().Length == 0 && info.GetDirectories().Length == 0;
        }

        public static string ToRegex(this string str)
        {
            List<char> chars = str.ToCharArray().ToList();

            for (int i = 0; i < chars.Count; i++)
            {
                if (chars[i] >= 'a' && chars[i] <= 'z' ||
                    chars[i] >= 'A' && chars[i] <= 'Z' ||
                    chars[i] >= '0' && chars[i] <= '9' ||
                    chars[i] == '_')
                {
                    continue;
                }
                else
                {
                    throw new InvalidDataException(str);
                }
            }

            return new string(chars.ToArray());
        }
    }
}
