using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentRefLoader
{
    public static class ReferenceLoaderSettingsFactory
    {
        public static IReferenceLoaderSettings GetSettings(this ReferenceLoaderStrategy strategy)
        {
            switch (strategy)
            {
                case ReferenceLoaderStrategy.RawCopy: return new DefaultSettings();
                case ReferenceLoaderStrategy.RawCopyNoRemote: return new RawCopyNoRemoteSettings();
                case ReferenceLoaderStrategy.OpenApiV2Merge: return new OpenApiV2MergeSettigns();
                default: throw new NotImplementedException($"{strategy} settings strategy not implemented");
            }
        }
    }
}
