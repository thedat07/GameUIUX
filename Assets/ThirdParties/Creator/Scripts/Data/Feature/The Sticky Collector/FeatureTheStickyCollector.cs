using UnityEngine;
using UnityUtilities;
using System;
using UniRx;
using Core;

public partial class FeatureTheStickyCollector : FeatureData, IFeatureUpdatable
{
    public class Data
    {
        private const string KEY_CLAIM_INDEX = "claimIndex";
        private const string KEY_COlOR = "color";
        private const string KEY_AMOUNT = "amount";
        private const string KEY_CURRENT_WEEK_START = "currentWeekStart";
        private const string KEY_CURRENT_WEEK_END = "currentWeekEnd";

        public IntReactiveProperty claimIndex;
        public IntReactiveProperty amount;
        public StringReactiveProperty currentWeekStart;
        public StringReactiveProperty currentWeekEnd;
        public ReactiveProperty<GameColor> currentColor = new ReactiveProperty<GameColor>(GameColor.Red);

        DateTime GetWeekStart() => DateTimeExtensions.FindFirstDateOfTheWeek(NetworkTime.UTC).Date;
        DateTime GetWeekEnd() => DateTimeExtensions.FindLastDateOfTheWeek(NetworkTime.UTC).Date.AddDays(1).AddTicks(-1);

        public Data()
        {
            claimIndex = new IntReactiveProperty(0);
            amount = new IntReactiveProperty(0);
            currentWeekStart = new StringReactiveProperty(GetWeekStart().ToString());
            currentWeekEnd = new StringReactiveProperty(GetWeekEnd().ToString());
            currentColor = new ReactiveProperty<GameColor>(GameColor.Red);
        }

        public void Reset()
        {
            claimIndex.Value = 0;
            amount.Value = 0;
            currentWeekStart.Value = GetWeekStart().ToString();
            currentWeekEnd.Value = GetWeekEnd().ToString();
            currentColor.Value = HelperCreator.GetRandomBrightColor();
        }

        public void Save(TypeFeature type)
        {
            SaveExtensions.PutFeature(type, KEY_CLAIM_INDEX, claimIndex.Value);
            SaveExtensions.PutFeature(type, KEY_AMOUNT, amount.Value);
            SaveExtensions.PutFeature(type, KEY_CURRENT_WEEK_START, currentWeekStart.Value);
            SaveExtensions.PutFeature(type, KEY_CURRENT_WEEK_END, currentWeekEnd.Value);
            SaveExtensions.PutFeature(type, KEY_COlOR, currentColor.Value);
        }

        public void Load(TypeFeature type)
        {
            claimIndex.Value = SaveExtensions.GetFeature(type, KEY_CLAIM_INDEX, 0);
            amount.Value = SaveExtensions.GetFeature(type, KEY_AMOUNT, 0);
            currentWeekStart.Value = SaveExtensions.GetFeature(type, KEY_CURRENT_WEEK_START, GetWeekStart().ToString());
            currentWeekEnd.Value = SaveExtensions.GetFeature(type, KEY_CURRENT_WEEK_END, GetWeekEnd().ToString());
            currentColor.Value = SaveExtensions.GetFeature(type, KEY_COlOR, GameColor.Red);
        }
    }

    Data m_Data = new Data();

    public Data GetData() => m_Data;

    public GameColor GetColor() => m_Data.currentColor.Value;

    public bool IsMax() => m_Data.claimIndex.Value >= 25;

    public FeatureTheStickyCollector(TypeFeature type, int levelUnlock = 0) : base(type, levelUnlock)
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