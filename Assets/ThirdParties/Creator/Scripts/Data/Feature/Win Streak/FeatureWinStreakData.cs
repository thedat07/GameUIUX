using UnityUtilities;
using UniRx;
using System.Collections.Generic;
using System;

public partial class FeatureWinStreak
{
    public class Data
    {
        public int EventDuration
        {
            get
            {
                return RemoteConfigController.GetIntConfig("feature_win_streak_duration", 7);
            }
        }

        public int EventInterval
        {
            get
            {
                return RemoteConfigController.GetIntConfig("feature_win_streak_interval", 1);
            }
        }

        private const string KEY_CURRENT_STREAK = "currentStreak";
        private const string KEY_CURRENT_STREAK_CLAIM = "currentClaim";
        private const string KEY_CURRENT_ID_CLAIM = "currentIdClaim";

        public IntReactiveProperty currentStreak;
        public IntReactiveProperty currentStreakClaim;
        public ReactiveCollection<int> currentIdClaim;
        public FeatureDataEventTime featureDataEventTime;

        public Data()
        {
            currentStreak = new IntReactiveProperty(0);
            currentStreakClaim = new IntReactiveProperty(0);
            currentIdClaim = new ReactiveCollection<int>();
            featureDataEventTime = new FeatureDataEventTime(EventDuration, EventInterval);
        }

        public void Reset()
        {
            if (!NetworkTime.ActiveNetworkTime) return;
            currentStreak.Value = 0;
            currentStreakClaim.Value = 0;
            currentIdClaim.Clear();
        }

        public void Save(TypeFeature type)
        {
            SaveExtensions.PutFeature(type, KEY_CURRENT_STREAK, currentStreak.Value);
            SaveExtensions.PutFeature(type, KEY_CURRENT_STREAK_CLAIM, currentStreakClaim.Value);
            SaveExtensions.PutFeature(type, KEY_CURRENT_ID_CLAIM, new List<int>(currentIdClaim));
        }

        public void Load(TypeFeature type)
        {
            currentStreak.Value = SaveExtensions.GetFeature(type, KEY_CURRENT_STREAK, 0);
            currentStreakClaim.Value = SaveExtensions.GetFeature(type, KEY_CURRENT_STREAK_CLAIM, 0);

            var list = SaveExtensions.GetFeature(type, KEY_CURRENT_ID_CLAIM, new List<int>());
            currentIdClaim.Clear();
            foreach (var id in list)
                currentIdClaim.Add(id);
        }

        public void AddIdClaim(int id)
        {
            if (!currentIdClaim.Contains(id))
                currentIdClaim.Add(id);
        }

        public bool HasIdClaim(int id)
        {
            return currentIdClaim.Contains(id);
        }

        public void RemoveIdClaim(int id)
        {
            currentIdClaim.Remove(id);
        }

        public bool IsReset()
        {
            return currentStreak.Value != 0 ||
                 currentStreakClaim.Value != 0 ||
                 currentIdClaim.Count > 0;
        }
    }

    private Data m_Data = new Data();

    public Data GetData() => m_Data;

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