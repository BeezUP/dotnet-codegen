using System;

namespace DocumentRefLoader
{
    public class RefInfo
    {
        static public RefInfo GetRefInfo(string documentUri, string refPath) => GetRefInfo(documentUri.GetAbsoluteUri(), refPath);

        static public RefInfo GetRefInfo(Uri documentUri, string refPath)
        {
            documentUri = documentUri.GetAbsolute();
            var documentFolder = documentUri.GetFolder();

            var parts = refPath.Split('#');
            if (parts.Length > 2)
                throw new ArgumentException(Constants.REF_KEYWORD + " value should not have more than 1 '#' char.", nameof(refPath));

            (bool embeded, string doc, string path, bool falseAbsoluteRef) refParts;
            if (parts.Length == 1) // uri only
            {
                refParts = (false, parts[0], "", false);
            }
            else if (string.IsNullOrWhiteSpace(parts[0]))
            {
                refParts = (true, "", parts[1], false);
            }
            else if (string.Compare(parts[0], documentUri.ToString(), StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                refParts = (true, parts[0], parts[1], true);
            }
            else
            {
                refParts = (false, parts[0], parts[1], false);
            }

            if (refParts.embeded)
                return new RefInfo(true, documentUri.IsFile, documentUri, refParts.path, refParts.falseAbsoluteRef);

            var uri = new Uri(refParts.doc, UriKind.RelativeOrAbsolute);
            if (!uri.IsAbsoluteUri)
                uri = new Uri(documentFolder, uri);

            return new RefInfo(false, uri.IsFile, uri, refParts.path, refParts.falseAbsoluteRef);
        }

        internal RefInfo(bool isNested, bool isLocal, Uri absoluteUri, string path, bool isFalseAbsoluteRef)
        {
            IsNestedInThisDocument = isNested;
            IsLocal = isLocal;
            AbsoluteDocumentUri = absoluteUri;
            InDocumentPath = path;
            IsFalseAbsoluteRef = isFalseAbsoluteRef;
        }

        public bool IsNestedInThisDocument { get; }
        public bool IsLocal { get; }
        public Uri AbsoluteDocumentUri { get; }
        public string InDocumentPath { get; }
        public bool IsFalseAbsoluteRef { get; }
    }
}
