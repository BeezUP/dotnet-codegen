namespace DocumentRefLoader.Settings
{
    public class RefContentCopyNoRemoteSettings : DefaultSettings
    {
        public override bool ShouldResolveReference(RefInfo refInfo, ResolveRefState state) => refInfo.IsLocal;
    }
}
