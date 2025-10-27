using System.Collections.Generic;
using System.Linq;
using Creator;
using DG.Tweening;
using Lean.Pool;
using TMPro;
using UnityEngine;
using UnityTimer;

public partial class HomeController
{
    public void ShowPopupFirst()
    {
        if (m_Data.win)
        {
            List<TypeFeature> data = StaticDataFeature.GetUnlockedActiveFeatures();
            if (data.Count > 0)
                ShowPopupFeature(data.First());
        }
    }

    void ShowPopupFeature(TypeFeature type)
    {
        switch (type)
        {
            case TypeFeature.PiggyBank:
                OnPiggy();
                break;
            case TypeFeature.DailyRewardsAds:
                OnDailyRewards();
                break;
            case TypeFeature.DailyRewardsFree:
                OnDailyRewards();
                break;
            case TypeFeature.LuckySpin:
                OnSpin();
                break;
            case TypeFeature.RemoveAds:
                OnRemoveAds();
                break;
            case TypeFeature.StarterPack:
                OnStarterPack();
                break;
            case TypeFeature.TheStickyCollector:
                OnTSC();
                break;
        }
    }
}