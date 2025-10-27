using UnityUtilities;
using UniRx;
using System.Collections.Generic;
using System;

public partial class FeatureWinStreak : FeatureDataRemoteConfig, IFeatureUpdatable
{
    public int GetCurrentStreak() => m_Data.currentStreak.Value;

    public FeatureWinStreak(TypeFeature type, int levelUnlock = 0) : base(type, levelUnlock)
    {
        LoadData();
        CheckTimeReset();
    }
}