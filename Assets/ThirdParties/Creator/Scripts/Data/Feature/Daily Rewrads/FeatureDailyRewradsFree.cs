using UnityEngine;
using UnityUtilities;
using System;
using UniRx;

public class FeatureDailyRewradsFree : FeatureData, IFeatureUpdatable
{
    public class Data
    {
        private const string KEY_LAST_CLAIM_TIME = "lastClaimTime";
        private const string KEY_IS_CLAIM = "isClaim";

        public StringReactiveProperty lastClaimTime;
        public BoolReactiveProperty isClaim;

        public Data()
        {
            lastClaimTime = new StringReactiveProperty(NetworkTime.UTC.ToString());
            isClaim = new BoolReactiveProperty(false);
        }

        public void Reset(bool claim = false)
        {
            lastClaimTime.Value = NetworkTime.UTC.ToString();
            isClaim.Value = claim;
        }

        public void Save(TypeFeature type)
        {
            SaveExtensions.PutFeature(type, KEY_LAST_CLAIM_TIME, lastClaimTime.Value);
            SaveExtensions.PutFeature(type, KEY_IS_CLAIM, isClaim.Value);
        }

        public void Load(TypeFeature type)
        {
            isClaim.Value = SaveExtensions.GetFeature(type, KEY_IS_CLAIM, false);
            lastClaimTime.Value = SaveExtensions.GetFeature(type, KEY_LAST_CLAIM_TIME, NetworkTime.UTC.ToString());
        }
    }


    private Data m_Data = new Data();

    public int CountNoti() => m_Data.isClaim.Value ? 0 : 1;

    public Data GetData() => m_Data;

    private DateTime GetNextAvailable()
    {
        DateTime lastClaim = DateTime.Parse(m_Data.lastClaimTime.Value);
        DateTime nextAvailable = lastClaim.AddHours(StaticData.TimeCooldownHoursDaily);
        return nextAvailable;
    }

    public FeatureDailyRewradsFree(TypeFeature type, int timeCooldownHours, int levelUnlock = 0) : base(type, levelUnlock)
    {
        LoadData();
        CheckAndResetIfNeeded();
    }

    public bool Claim()
    {
        if (!CanClaim()) return false;
        m_Data.Reset(true);
        SaveData();
        return true;
    }

    public bool CanClaim()
    {
        CheckAndResetIfNeeded();

        if (!m_Data.isClaim.Value)
            return true;

        return false;
    }


    public TimeSpan GetRemainingTime()
    {
        if (string.IsNullOrEmpty(m_Data.lastClaimTime.Value))
            return TimeSpan.Zero;

        TimeSpan remain = GetNextAvailable() - NetworkTime.UTC;
        return remain.TotalSeconds > 0 ? remain : TimeSpan.Zero;
    }

    private void CheckAndResetIfNeeded()
    {
        if (string.IsNullOrEmpty(m_Data.lastClaimTime.Value)) return;

        if (NetworkTime.UTC >= GetNextAvailable())
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
        CheckAndResetIfNeeded();
    }
}