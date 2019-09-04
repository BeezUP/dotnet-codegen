using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentRefLoader
{
    public class ResolveRefState
    {
        public static ResolveRefState Default() => new ResolveRefState();

        public static ResolveRefState FromParent(ResolveRefState parent)
        {
            return new ResolveRefState
            {
                HasEnteredNonNestedReference = parent.HasEnteredNonNestedReference,
            };
        }

        private ResolveRefState() { }

        public bool HasEnteredNonNestedReference { get; private set; } = false;

        internal void HandleRefInfo(RefInfo refInfo)
        {
            if (HasEnteredNonNestedReference) return;
            HasEnteredNonNestedReference = !refInfo.IsNestedInThisDocument;
        }
    }
}
