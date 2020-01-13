using System;

namespace DocumentRefLoader
{
    public class OpenApiRefInfo
    {
        public static OpenApiRefInfo GetRefInfo(Uri documentUri, Uri documentFolder, string refPath)
        {
            var parts = refPath.Split('#');
            if (parts.Length > 2)
                throw new ArgumentException(Constants.REF_KEYWORD + " value should not have more than 1 '#' char.", nameof(refPath));

            (bool embeded, string doc, string path) refParts;
            if (parts.Length == 1) // uri only
            {
                refParts = (false, parts[0], "");
            }
            else if (string.IsNullOrWhiteSpace(parts[0]))
            {
                refParts = (true, "", parts[1]);
            }
            else
            {
                refParts = (false, parts[0], parts[1]);
            }

            if (refParts.embeded)
                return new OpenApiRefInfo(true, documentUri.IsFile, documentUri, refParts.path);

            var uri = new Uri(refParts.doc, UriKind.RelativeOrAbsolute);
            if (!uri.IsAbsoluteUri)
                uri = new Uri(documentFolder, uri);

            return new OpenApiRefInfo(false, uri.IsFile, uri, refParts.path);
        }

        public OpenApiRefInfo(bool isNested, bool isLocal, Uri absoluteUri, string path)
        {
            IsNestedInThisDocument = isNested;
            IsLocal = isLocal;
            AbsoluteDocumentUri = absoluteUri;
            InDocumentPath = path;
        }

        public bool IsNestedInThisDocument { get; }
        public bool IsLocal { get; }
        public Uri AbsoluteDocumentUri { get; }
        public string InDocumentPath { get; }


    }

}