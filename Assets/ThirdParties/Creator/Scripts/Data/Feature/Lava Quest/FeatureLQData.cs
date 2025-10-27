using UnityUtilities;
using UniRx;
using System.Collections.Generic;
using System;

public partial class FeatureLavaQuest
{
    public class Data
    {
        List<(int min, int max)> ranges = new List<(int, int)>
        {
            (100, 100),
            (72, 88),
            (55, 65),
            (40, 50),
            (30, 36),
            (24, 26),
            (17, 22),
            (11, 15),
        };

        public int EventDuration
        {
            get
            {
                return RemoteConfigController.GetIntConfig("feature_lava_quest_duration", 24);
            }
        }

        public int EventInterval
        {
            get
            {
                return RemoteConfigController.GetIntConfig("feature_lava_quest_interval", 15);
            }
        }

        private const string KEY_CURRENT_STREAK = "currentStreak";

        private const string KEY_EVENT_START_TIME = "eventStartTime";

        private const string KEY_CURRENT_INTERVAL = "currentInterval";

        private const string KEY_CURRENT_PLAYER = "currentPlayer";

        public IntReactiveProperty currentStreak;
        public BoolReactiveProperty currentInterval;
        public StringReactiveProperty eventStartTime;
        public ReactiveCollection<int> usersLeftByLevel;

        public DateTime EventEndTime => DateTime.Parse(eventStartTime.Value).AddHours(EventDuration);

        public DateTime GetTimeCooldownEvent()
        {
            if (currentInterval.Value == false)
                return DateTime.Parse(eventStartTime.Value).AddHours(EventDuration).AddMinutes(EventInterval);
            else
                return DateTime.Parse(eventStartTime.Value).AddMinutes(EventInterval);
        }

        public Data()
        {
            currentStreak = new IntReactiveProperty(0);
            eventStartTime = new StringReactiveProperty(NetworkTime.UTC.ToString());
            currentInterval = new BoolReactiveProperty(false);
            usersLeftByLevel = new ReactiveCollection<int>(RandomPlayer());
        }

        List<int> RandomPlayer()
        {
            List<int> usersLeftByLevel = new List<int>();

            for (int i = 0; i < ranges.Count; i++)
            {
                int rand = UnityEngine.Random.Range(ranges[i].min, ranges[i].max + 1);
                usersLeftByLevel.Add(rand);
            }
            return usersLeftByLevel;
        }

        public void Save(TypeFeature type)
        {
            SaveExtensions.PutFeature(type, KEY_CURRENT_STREAK, currentStreak.Value);
            SaveExtensions.PutFeature(type, KEY_EVENT_START_TIME, eventStartTime.Value);
            SaveExtensions.PutFeature(type, KEY_CURRENT_INTERVAL, currentInterval.Value);
            SaveExtensions.PutFeature(type, KEY_CURRENT_PLAYER, new List<int>(usersLeftByLevel));
        }

        public void Load(TypeFeature type)
        {
            currentStreak.Value = SaveExtensions.GetFeature(type, KEY_CURRENT_STREAK, 0);
            eventStartTime.Value = SaveExtensions.GetFeature(type, KEY_EVENT_START_TIME, NetworkTime.UTC.ToString());
            currentInterval.Value = SaveExtensions.GetFeature(type, KEY_CURRENT_INTERVAL, false);

            var listFree = SaveExtensions.GetFeature(type, KEY_CURRENT_PLAYER, new List<int>());
            usersLeftByLevel.Clear();
            foreach (var id in listFree)
                usersLeftByLevel.Add(id);
        }

        public void Reset()
        {
            currentInterval.Value = false;
            currentStreak.Value = 0;
            eventStartTime.Value = NetworkTime.UTC.ToString();
            usersLeftByLevel = new ReactiveCollection<int>(RandomPlayer());
        }

        public void SetInterval()
        {
            currentInterval.Value = true;
            Reset();
        }

        public bool IsReset()
        {
            return currentStreak.Value != 0;
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

    public int GetRewards()
    {
        if (m_Data.currentStreak.Value == 0)
        {
            return 0;
        }
        return StaticData.LavaQuestMoney / m_Data.currentStreak.Value;
    }
}