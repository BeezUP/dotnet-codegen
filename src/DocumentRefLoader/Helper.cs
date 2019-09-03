using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using YamlDotNet.Serialization;

namespace DocumentRefLoader
{
    public static class Helper
    {
        readonly static Uri SomeBaseUri = new Uri("http://canbeanything");

        /// <summary>
        /// Stackoverflow coding : https://stackoverflow.com/questions/1105593/get-file-name-from-uri-string-in-c-sharp
        /// </summary>
        internal static string GetFileNameFromUrl(string url)
        {
            Uri uri;
            if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
                uri = new Uri(SomeBaseUri, url);

            return Path.GetFileName(uri.LocalPath);
        }
    }
}
