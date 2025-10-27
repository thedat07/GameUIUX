using UnityUtilities;
using UniRx;

public partial class FeatureLavaQuest : FeatureDataRemoteConfig, IFeatureUpdatable
{
    public int GetCurrentStreak() => m_Data.currentStreak.Value;

    public FeatureLavaQuest(TypeFeature type, int levelUnlock = 0) : base(type, levelUnlock)
    {
        LoadData();
        CheckTimeReset();
    }
}