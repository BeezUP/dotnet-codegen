using System;

namespace DocumentRefLoader.Settings
{
    public static class ReferenceLoaderSettingsFactory
    {
        public static IReferenceLoaderSettings GetSettings(this ReferenceLoaderStrategy strategy)
        {
            switch (strategy)
            {
                case ReferenceLoaderStrategy.CopyRefContent: return new DefaultSettings();
                case ReferenceLoaderStrategy.RefContentCopyNoRemote: return new RefContentCopyNoRemoteSettings();
                default: throw new NotImplementedException($"{strategy} settings strategy not implemented");
            }
        }
    }
}
