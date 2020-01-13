using System;

namespace DocumentRefLoader.Settings
{
    public static class ReferenceLoaderSettingsFactory
    {
        public static IReferenceLoaderSettings GetSettings(this ReferenceLoaderStrategy strategy)
        {
            switch (strategy)
            {
                case ReferenceLoaderStrategy.RefContentCopy: return new DefaultSettings();
                case ReferenceLoaderStrategy.RefContentCopyNoRemote: return new RefContentCopyNoRemoteSettings();
                case ReferenceLoaderStrategy.OpenApiV2Merge: return new OpenApiV2MergeSettings();
                default: throw new NotImplementedException($"{strategy} settings strategy not implemented");
            }
        }
    }
}
