using UnityUtilities;

public partial class FeatureSeasonPass : FeatureDataRemoteConfig, IFeatureUpdatable
{
    public bool IsVip() => m_Data.activeVip.Value;

    public bool IsActiveEvent() => IsDuringEvent() && IsUnlock();

    public FeatureSeasonPass(TypeFeature type, int levelUnlock = 0) : base(type, levelUnlock)
    {
        LoadData();
        CheckTimeReset();
    }
}