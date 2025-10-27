using UnityEngine;
using UnityUtilities;
using System;
using UniRx;

public partial class FeatureStarterPack : FeatureData, IFeatureUpdatable
{
    public class Data
    {
        private const string KEY_STAGE = "stage";
        private const string KEY_START_TIME = "startTime";
        private const string KEY_IS_SHOWING = "isShowing";

        public IntReactiveProperty stage;
        public BoolReactiveProperty isShowing;
        public StringReactiveProperty startTime;

        public DateTime GetStartTime() => DateTime.Parse(startTime.Value);

        public Data()
        {
            stage = new IntReactiveProperty(0);
            isShowing = new BoolReactiveProperty(true);
            startTime = new StringReactiveProperty(NetworkTime.UTC.ToString());
        }

        public void Save(TypeFeature type)
        {
            SaveExtensions.PutFeature(type, KEY_STAGE, stage.Value);
            SaveExtensions.PutFeature(type, KEY_START_TIME, isShowing.Value);
            SaveExtensions.PutFeature(type, KEY_IS_SHOWING, startTime.Value);
        }

        public void Load(TypeFeature type)
        {
            stage.Value = SaveExtensions.GetFeature(type, KEY_STAGE, 0);
            isShowing.Value = SaveExtensions.GetFeature(type, KEY_START_TIME, true);
            startTime.Value = SaveExtensions.GetFeature(type, KEY_IS_SHOWING, NetworkTime.UTC.ToString());
        }
    }

    Data m_Data = new Data();

    public bool IsActive() => !GameManager.Instance.GetShopModelView().HasReceivedReward(Gley.EasyIAP.ShopProductNames.StarterPack.ToString());

    private readonly TimeSpan[] stageDurations = new TimeSpan[]
    {
        TimeSpan.FromHours(24),
        TimeSpan.FromHours(12),
        TimeSpan.FromHours(8),
        TimeSpan.FromHours(6),
        TimeSpan.FromHours(4),
        TimeSpan.FromHours(2),
        TimeSpan.FromHours(1),
        TimeSpan.FromHours(1),
    };

    public FeatureStarterPack(TypeFeature type, int levelUnlock = 0) : base(type, levelUnlock)
    {
        LoadData();
        CheckTimeReset();
    }

    private TimeSpan GetCurrentDuration()
    {
        return stageDurations[Mathf.Clamp(m_Data.stage.Value, 0, stageDurations.Length - 1)];
    }

    public bool IsShowing()
    {
        return m_Data.isShowing.Value && IsActive();
    }

    public TimeSpan GetRemainingTime()
    {
        if (!m_Data.isShowing.Value) return TimeSpan.Zero;
        var duration = GetCurrentDuration();
        var elapsed = NetworkTime.UTC - m_Data.GetStartTime();
        return duration - elapsed;
    }

    private void LoadData()
    {
        m_Data.Load(m_Type);
    }

    private void SaveData()
    {
        m_Data.Save(m_Type);
    }

    public void CheckTimeReset()
    {
        if (IsActive())
        {
            DateTime now = NetworkTime.UTC;

            if (m_Data.isShowing.Value)
            {
                TimeSpan duration = GetCurrentDuration();
                if (now - m_Data.GetStartTime() >= duration)
                {
                    m_Data.isShowing.Value = false;
                    m_Data.startTime.Value = NetworkTime.UTC.ToString();
                    SaveData();
                }
            }
            else
            {

                if (now - m_Data.GetStartTime() >= TimeSpan.FromDays(1))
                {
                    m_Data.stage.Value = Mathf.Min(m_Data.stage.Value + 1, stageDurations.Length - 1);
                    m_Data.isShowing.Value = true;
                    m_Data.startTime.Value = NetworkTime.UTC.ToString();
                    SaveData();
                }
            }
        }
    }

    public void CustomUpdate()
    {
        CheckTimeReset();
    }
}