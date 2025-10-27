using UnityEngine;
using UnityUtilities;
using System;
using UniRx;
using System.Collections.Generic;
using Core;

public partial class FeatureSeasonPass
{
    public class Data
    {
        public int EventDuration
        {
            get
            {
                return RemoteConfigController.GetIntConfig("feature_season_pass_duration", 30);
            }
        }

        public int EventInterval
        {
            get
            {
                return RemoteConfigController.GetIntConfig("feature_season_pass_interval", 1);
            }
        }

        private const string KEY_ACTIVE_VIP = "activeVip";
        private const string KEY_CLAIM_INDEX = "claimIndex";
        private const string KEY_CLAIM_INDEX_FREE = "claimIndexFree";
        private const string KEY_CLAIM_INDEX_VIP = "claimIndexVip";
        private const string KEY_EXP = "exp";

        public IntReactiveProperty exp;
        public BoolReactiveProperty activeVip;
        public IntReactiveProperty claimIndex;
        public ReactiveCollection<int> claimIndexFree;
        public ReactiveCollection<int> claimIndexVip;

        public FeatureDataEventTime featureDataEventTime;

        public bool IsReset()
        {
            return activeVip.Value != false ||
                   claimIndex.Value != 0 ||
                   claimIndexFree.Count > 0 ||
                   claimIndexVip.Count > 0;
        }

        public Data()
        {
            exp = new IntReactiveProperty(0);
            activeVip = new BoolReactiveProperty(false);
            claimIndex = new IntReactiveProperty(0);
            claimIndexFree = new ReactiveCollection<int>();
            claimIndexVip = new ReactiveCollection<int>();
            featureDataEventTime = new FeatureDataEventTime(EventDuration, EventInterval);
        }

        public void Reset()
        {
            if (!NetworkTime.ActiveNetworkTime) return;
            activeVip.Value = false;
            claimIndex.Value = 0;
            claimIndexFree.Clear();
            claimIndexVip.Clear();
            exp.Value = 0;
        }

        public void Save(TypeFeature type)
        {
            SaveExtensions.PutFeature(type, KEY_ACTIVE_VIP, activeVip.Value);
            SaveExtensions.PutFeature(type, KEY_CLAIM_INDEX, claimIndex.Value);
            SaveExtensions.PutFeature(type, KEY_EXP, exp.Value);

            SaveExtensions.PutFeature(type, KEY_CLAIM_INDEX_FREE, new List<int>(claimIndexFree));
            SaveExtensions.PutFeature(type, KEY_CLAIM_INDEX_VIP, new List<int>(claimIndexVip));
        }

        public void Load(TypeFeature type)
        {
            exp.Value = SaveExtensions.GetFeature(type, KEY_EXP, 0);

            activeVip.Value = SaveExtensions.GetFeature(type, KEY_ACTIVE_VIP, false);
            claimIndex.Value = SaveExtensions.GetFeature(type, KEY_CLAIM_INDEX, 0);

            var listFree = SaveExtensions.GetFeature(type, KEY_CLAIM_INDEX_FREE, new List<int>());
            claimIndexFree.Clear();
            foreach (var id in listFree)
                claimIndexFree.Add(id);

            var listVip = SaveExtensions.GetFeature(type, KEY_CLAIM_INDEX_VIP, new List<int>());
            claimIndexVip.Clear();
            foreach (var id in listVip)
                claimIndexVip.Add(id);
        }

        // Helper
        public void AddClaimFree(int index)
        {
            if (!claimIndexFree.Contains(index))
                claimIndexFree.Add(index);
        }

        public void AddClaimVip(int index)
        {
            if (!claimIndexVip.Contains(index))
                claimIndexVip.Add(index);
        }

        public bool HasClaimFree(int index)
        {
            // chỉ được claim nếu index <= tiến độ hiện tại
            if (index > claimIndex.Value) return false;
            return claimIndexFree.Contains(index);
        }

        public bool HasClaimVip(int index)
        {
            if (index > claimIndex.Value) return false;
            return claimIndexVip.Contains(index);
        }
    }

    Data m_Data = new Data();

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