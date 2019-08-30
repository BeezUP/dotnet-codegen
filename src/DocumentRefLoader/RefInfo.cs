using System;

namespace DocumentRefLoader
{
    public class RefInfo
    {
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
