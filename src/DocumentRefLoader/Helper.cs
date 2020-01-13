using System;
using System.IO;

namespace DocumentRefLoader
{
    public static class Helper
    {
        private const string SOME_BASE_URI = "http://canbeanything";
        readonly static Uri SomeBaseUri = new Uri(SOME_BASE_URI);

        /// <summary>
        /// Stackoverflow coding : https://stackoverflow.com/questions/1105593/get-file-name-from-uri-string-in-c-sharp
        /// </summary>
        internal static string GetFileNameFromUrl(string url)
        {
            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
                uri = new Uri(SomeBaseUri, url);

            return Path.GetFileName(uri.LocalPath);
        }
    }
}
