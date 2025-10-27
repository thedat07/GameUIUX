using System;
using UnityUtilities;
using UniRx;
using UnityEngine;

public class FeaturePiggyBank : FeatureData, IFeatureUpdatable
{
    public class Data
    {
        private const string KEY_EXP = "exp";
        private const string KEY_IS_FULL = "isFull";
        private const string KEY_FULL_TIMESTAMP = "fullTimeStamp";

        public IntReactiveProperty exp;
        public BoolReactiveProperty isFull;
        public StringReactiveProperty fullTimeStamp;

        public Data()
        {
            exp = new IntReactiveProperty(0);
            isFull = new BoolReactiveProperty(false);
            fullTimeStamp = new StringReactiveProperty("");
        }

        public void Reset()
        {
            exp.Value = 0;
            isFull.Value = false;
            fullTimeStamp.Value = "";
        }

        public void Save(TypeFeature type)
        {
            SaveExtensions.PutFeature(type, KEY_EXP, exp.Value);
            SaveExtensions.PutFeature(type, KEY_IS_FULL, isFull.Value);
            SaveExtensions.PutFeature(type, KEY_FULL_TIMESTAMP, fullTimeStamp.Value);
        }

        public void Load(TypeFeature type)
        {
            exp.Value = SaveExtensions.GetFeature(type, KEY_EXP, 0);
            isFull.Value = SaveExtensions.GetFeature(type, KEY_IS_FULL, false);
            fullTimeStamp.Value = SaveExtensions.GetFeature(type, KEY_FULL_TIMESTAMP, "");
        }
    }

    private int m_ExpPerWin = 100;

    private int m_MaxExp = 1000;

    private int m_CountdownHours = RemoteConfigController.GetIntConfig("feature_piggy_duration", 48);

    private Data m_Data = new Data();

    public Data GetData() => m_Data;

    public bool IsNoti() => m_Data.isFull.Value && m_Data.exp.Value >= m_MaxExp;

    public DateTime GetExpireTime()
    {
        DateTime fullTime = DateTime.Parse(m_Data.fullTimeStamp.Value);
        DateTime expireTime = fullTime.AddHours(m_CountdownHours);
        return expireTime;
    }

    public FeaturePiggyBank(TypeFeature type, int levelUnlock = 0) : base(type, levelUnlock)
    {
        LoadData();
        CheckExpire();
    }

    public void AddExpOnWin()
    {
        if (m_Data.isFull.Value) return;

        m_Data.exp.Value += m_ExpPerWin;
        if (m_Data.exp.Value >= m_MaxExp)
        {
            m_Data.exp.Value = m_MaxExp;
            m_Data.isFull.Value = true;
            m_Data.fullTimeStamp.Value = NetworkTime.UTC.ToString();
        }
        SaveData();
    }

    public bool BuyPiggy(out int coin)
    {
        coin = 0;
        CheckExpire();

        if (!m_Data.isFull.Value) return false;
        coin = StaticData.PiggyMoney;

        ResetPiggy();

        return true;
    }

    public void CheckExpire()
    {
        if (m_Data.isFull.Value && !string.IsNullOrEmpty(m_Data.fullTimeStamp.Value))
        {
            if (NetworkTime.UTC >= GetExpireTime())
            {
                ResetPiggy();
            }
        }
    }

    public TimeSpan GetRemainingTime()
    {
        if (!m_Data.isFull.Value || string.IsNullOrEmpty(m_Data.fullTimeStamp.Value))
            return TimeSpan.Zero;

        TimeSpan remain = GetExpireTime() - NetworkTime.UTC;
        return remain;
    }

    private void ResetPiggy()
    {
        m_Data.Reset();
        SaveData();
    }

    public int GetCurrentExp() => m_Data.exp.Value;

    public int GetMaxExp() => m_MaxExp;

    public bool IsFull() => m_Data.isFull.Value;

    private void LoadData()
    {
        m_Data.Load(m_Type);
    }

    private void SaveData()
    {
        m_Data.Save(m_Type);
    }

    public bool IsAvailableRealtime()
    {
        return GetRemainingTime() > TimeSpan.Zero;
    }

    public void CustomUpdate()
    {
        CheckExpire();
    }
}