namespace DocumentRefLoader.Settings
{
    public class RawCopyNoRemoteSettings : DefaultSettings
    {
        public override bool ShouldResolveReference(RefInfo refInfo, ResolveRefState state) => refInfo.IsLocal;
    }
}
