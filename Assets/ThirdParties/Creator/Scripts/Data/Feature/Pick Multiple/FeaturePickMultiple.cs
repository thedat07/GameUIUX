using UnityEngine;
using UnityUtilities;
using System;
using UniRx;

public partial class FeaturePickMultiple : FeatureDataRemoteConfig, IFeatureUpdatable
{
    public class Data
    {
        public int EventDuration
        {
            get
            {
                return RemoteConfigController.GetIntConfig("feature_pick_multiple_duration", 3);
            }
        }

        public int EventInterval
        {
            get
            {
                return RemoteConfigController.GetIntConfig("feature_pick_multiple_interval", 1);
            }
        }

        private const string KEY_CLAIM_INDEX = "claimIndex";

        public IntReactiveProperty claimIndex;

        public FeatureDataEventTime featureDataEventTime;

        public bool IsReset() => claimIndex.Value != 0;

        public Data()
        {
            claimIndex = new IntReactiveProperty(0);
            featureDataEventTime = new FeatureDataEventTime(EventDuration, EventInterval);
        }

        public void Reset()
        {
            if (!NetworkTime.ActiveNetworkTime) return;
            claimIndex.Value = 0;
        }

        public void Save(TypeFeature type)
        {
            SaveExtensions.PutFeature(type, KEY_CLAIM_INDEX, claimIndex.Value);
        }

        public void Load(TypeFeature type)
        {
            claimIndex.Value = SaveExtensions.GetFeature(type, KEY_CLAIM_INDEX, 0);
        }
    }

    Data m_Data = new Data();

    public bool IsActiveEvent() => IsDuringEvent() && IsUnlock();

    public FeaturePickMultiple(TypeFeature type, int levelUnlock = 0) : base(type, levelUnlock)
    {
        LoadData();
        CheckTimeReset();
    }

    private void LoadData()
    {
        m_Data.Load(m_Type);
    }

    private void DeleteData()
    {
        m_Data.Reset();
        SaveData();
    }

    private void SaveData()
    {
        m_Data.Save(m_Type);
    }
}