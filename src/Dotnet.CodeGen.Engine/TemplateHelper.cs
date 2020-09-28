using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Dotnet.CodeGen.CodeGen
{
    public static class TemplateHelper
    {

        public static IEnumerable<TemplateInfos> GetTemplates(string path, string extension = "*.handlebars")
        {
            if (Directory.Exists(path))
                return Directory.GetFiles(path, extension, SearchOption.AllDirectories).Select(p => GetDataFromPath(p, path, false));

            if (File.Exists(path))
                return new[] { GetDataFromPath(path, path, true) };

            return new TemplateInfos[0];
        }

        static TemplateInfos GetDataFromPath(string filePath, string rootPath, bool rootIsFile)
        {
            var fileName = Path.GetFileNameWithoutExtension(filePath);

            string directory;
            if (rootIsFile)
            {
                directory = "";
            }
            else
            {
                // directory = Path.GetDirectoryName(path).Remove(0, rootPath.Length);

                var fInfo = new FileInfo(filePath);
                var rootDir = new Uri(Path.GetFullPath(rootPath));
                var fDir = new Uri(fInfo.Directory.FullName);
                directory = fDir.OriginalString.Replace(rootDir.OriginalString, "");
                directory = directory.Trim(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar });
            }

            return new TemplateInfos(filePath, fileName, directory);
        }
    }
}
