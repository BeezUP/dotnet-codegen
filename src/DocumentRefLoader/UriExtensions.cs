using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DocumentRefLoader
{
    public static class UriExtensions
    {
        static public Uri GetAbsoluteUri(this string uri)
            => new Uri(uri, UriKind.RelativeOrAbsolute).GetAbsolute();

        static public Uri GetAbsolute(this Uri uri)
            => uri.IsAbsoluteUri
                ? uri
                : new Uri(new Uri(Path.Combine(Directory.GetCurrentDirectory(), ".")), uri);

        static public Uri GetFolder(this Uri uri)
            => new Uri(uri.GetAbsolute(), ".");

        static public Task<string> DownloadDocumentAsync(this Uri uri, string authorization = null)
        {
            using (var webClient = new WebClient())
            {
                if (authorization != null)
                {
                    webClient.Headers.Add("Authorization", authorization);
                }
                return webClient.DownloadStringTaskAsync(uri);
            }
        }
    }
}
