using System;
using System.Collections.Generic;
using System.Text;

namespace Dotnet.CodeGen
{
    public class TemplateInfos
    {
        internal TemplateInfos(string filePath, string fileName, string directory)
        {
            FilePath = filePath;
            FileName = fileName;
            Directory = directory;
        }

        /// <summary>
        /// Full path of the file
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Expected file name (without the template extension)
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Expected relative directory (from the template root)
        /// </summary>
        public string Directory { get; }

        public override int GetHashCode()
        {
            return FilePath.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (!(obj is TemplateInfos)) return false;
            return obj.GetHashCode() == GetHashCode();
        }
    }
}
