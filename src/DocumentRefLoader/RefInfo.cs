using System;

namespace DocumentRefLoader
{
    public class RefInfo
    {
        public RefInfo(bool isNested, bool isLocal, Uri absoluteUri, string path)
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
