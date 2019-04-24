using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Dotnet.CodeGen.Misc
{

    [Serializable]
    [CollectionDataContract(ItemName = "Item", KeyName = "Key", ValueName = "Value")]
    public class CaseInsensitiveDictionary<TValue> : Dictionary<string, TValue>
    {
        public CaseInsensitiveDictionary() : base(StringComparer.InvariantCultureIgnoreCase) { }

        public CaseInsensitiveDictionary(IDictionary<string, TValue> dictionary) : base(dictionary, StringComparer.InvariantCultureIgnoreCase)
        {
        }

        protected CaseInsensitiveDictionary(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }

    public static class CaseInsensitiveDictionaryExtensions
    {
        public static CaseInsensitiveDictionary<TElement> ToCaseInsensitiveDictionary<TSource, TElement>(this IEnumerable<TSource> source, Func<TSource, string> keySelector, Func<TSource, TElement> elementSelector)
        {
            var dico = new CaseInsensitiveDictionary<TElement>();

            foreach (var item in source)
                dico[keySelector(item)] = elementSelector(item);

            return dico;
        }
    }
}
