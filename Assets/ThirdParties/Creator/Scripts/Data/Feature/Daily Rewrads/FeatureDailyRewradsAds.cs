using System;
using UnityUtilities;
using UniRx;

public class FeatureDailyRewradsAds : FeatureData, IFeatureUpdatable
{
    public class Data
    {
        private const string KEY_LAST_CLAIM_DATE = "lastClaimDate";
        private const string KEY_CLAIM_INDEX = "claimIndex";

        public StringReactiveProperty lastClaimDate;
        public IntReactiveProperty claimIndex;

        public Data()
        {
            lastClaimDate = new StringReactiveProperty(NetworkTime.UTC.Date.ToString());
            claimIndex = new IntReactiveProperty(0);
        }

        public void Reset()
        {
            lastClaimDate.Value = NetworkTime.UTC.Date.ToString();
            claimIndex.Value = 0;
        }

        public void Save(TypeFeature type)
        {
            SaveExtensions.PutFeature(type, KEY_LAST_CLAIM_DATE, lastClaimDate.Value);
            SaveExtensions.PutFeature(type, KEY_CLAIM_INDEX, claimIndex.Value);
        }

        public void Load(TypeFeature type)
        {
            claimIndex.Value = SaveExtensions.GetFeature(type, KEY_CLAIM_INDEX, 0);
            lastClaimDate.Value = SaveExtensions.GetFeature(type, KEY_LAST_CLAIM_DATE, NetworkTime.UTC.Date.ToString());
        }
    }


    private int m_MaxDailyRewards = 4;

    private Data m_Data = new Data();

    public int CountNoti() => m_MaxDailyRewards - m_Data.claimIndex.Value;

    public int GetClaimIndex() => m_Data.claimIndex.Value;

    public Data GetData() => m_Data;

    public FeatureDailyRewradsAds(TypeFeature type, int levelUnlock = 0) : base(type, levelUnlock)
    {
        LoadData();
        CheckDailyReset();
    }

    public bool CanRewards()
    {
        if (!IsUnlock())
            return false;

        CheckDailyReset();

        return m_Data.claimIndex.Value < m_MaxDailyRewards;
    }

    public void Claim()
    {
        m_Data.claimIndex.Value++;
        SaveData();
    }

    private void CheckDailyReset()
    {
        if (string.IsNullOrEmpty(m_Data.lastClaimDate.Value))
        {
            m_Data.Reset();
            SaveData();
        }

        string today = NetworkTime.UTC.Date.ToString();

        if (m_Data.lastClaimDate.Value != today)
        {
            m_Data.Reset();
            SaveData();
        }
    }

    private void LoadData()
    {
        m_Data.Load(m_Type);
    }

    private void SaveData()
    {
        m_Data.Save(m_Type);
    }

    public void CustomUpdate()
    {
        CheckDailyReset();
    }
}