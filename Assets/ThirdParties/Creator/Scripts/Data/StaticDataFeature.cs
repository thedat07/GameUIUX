using System.Collections.Generic;
using UnityEngine;
using UnityUtilities;

public enum TypeFeature
{
    LuckySpin = 1,
    PiggyBank = 2,
    TheStickyCollector = 3,
    DailyRewardsAds = 4,
    DailyRewardsFree = 5,
    PickMultiple = 6,
    RollingOffer = 7,
    SeasonPass = 8,
    WinStreak = 9,
    StarterPack = 10,
    RemoveAds = 11,
    FreeOffer = 12,
    LavaQuest = 13,
}

public static class StaticDataFeature
{
    public static Dictionary<TypeFeature, int> FeatureUnlockLevels { get; private set; }

    public static Dictionary<TypeFeature, bool> FeatureActive { get; private set; }

    static StaticDataFeature()
    {
        RefreshFeatureUnlockLevels();
        RefreshFeatureActive();
    }

    public static void Refresh()
    {
        RefreshFeatureUnlockLevels();
        RefreshFeatureActive();
    }

    public static void RefreshFeatureActive()
    {
        FeatureActive = new Dictionary<TypeFeature, bool>()
        {
            { TypeFeature.LuckySpin, RemoteConfigController.GetBoolConfig("feature_luck_spin_active", false) },
            { TypeFeature.PiggyBank, RemoteConfigController.GetBoolConfig("feature_piggy_active", false) },
            { TypeFeature.DailyRewardsAds, RemoteConfigController.GetBoolConfig("feature_daily_active", false) },
            { TypeFeature.DailyRewardsFree, RemoteConfigController.GetBoolConfig("feature_daily_active", false) },
            { TypeFeature.TheStickyCollector, RemoteConfigController.GetBoolConfig("feature_tsc_active", true) },
            { TypeFeature.PickMultiple, RemoteConfigController.GetBoolConfig("feature_pick_multiple_active", false) },
            { TypeFeature.RollingOffer, RemoteConfigController.GetBoolConfig("feature_rolling_offer_active", false) },
            { TypeFeature.SeasonPass, RemoteConfigController.GetBoolConfig("feature_season_pass_active", false) },
            { TypeFeature.WinStreak, RemoteConfigController.GetBoolConfig("feature_win_streak_active", false) },
            { TypeFeature.FreeOffer, RemoteConfigController.GetBoolConfig("feature_free_offer_active", false) },
            { TypeFeature.RemoveAds, !GameManager.Instance.GetShopModelView().HasReceivedReward(Gley.EasyIAP.ShopProductNames.RemoveAdsBundle.ToString())},
            { TypeFeature.StarterPack, !GameManager.Instance.GetShopModelView().HasReceivedReward(Gley.EasyIAP.ShopProductNames.StarterPack.ToString())},
            { TypeFeature.LavaQuest, RemoteConfigController.GetBoolConfig("feature_lava_quest_active", false) },
        };
    }

    public static void RefreshFeatureUnlockLevels()
    {
        FeatureUnlockLevels = new Dictionary<TypeFeature, int>()
        {
            { TypeFeature.LuckySpin, RemoteConfigController.GetIntConfig("feature_luck_spin_un_lock", 42) },
            { TypeFeature.PiggyBank, RemoteConfigController.GetIntConfig("feature_piggy_un_lock", 18) },
            { TypeFeature.DailyRewardsAds, RemoteConfigController.GetIntConfig("feature_daily_un_lock", 26) },
            { TypeFeature.DailyRewardsFree, RemoteConfigController.GetIntConfig("feature_daily_un_lock", 26) },
            { TypeFeature.TheStickyCollector, RemoteConfigController.GetIntConfig("feature_tsc_un_lock", 48) },
            { TypeFeature.PickMultiple, RemoteConfigController.GetIntConfig("feature_pick_multiple_un_lock", 80) },
            { TypeFeature.RollingOffer, RemoteConfigController.GetIntConfig("feature_rolling_offer_un_lock", 60) },
            { TypeFeature.SeasonPass, RemoteConfigController.GetIntConfig("feature_season_pass_un_lock", 37) },
            { TypeFeature.WinStreak, RemoteConfigController.GetIntConfig("feature_win_streak_un_lock", 50) },
            { TypeFeature.RemoveAds, RemoteConfigController.GetIntConfig("feature_remove_ads_show", 16) },
            { TypeFeature.StarterPack, RemoteConfigController.GetIntConfig("feature_starter_pack_show", 15) },
            { TypeFeature.FreeOffer, RemoteConfigController.GetIntConfig("feature_free_offer_un_lock", 999) },
            { TypeFeature.LavaQuest, RemoteConfigController.GetIntConfig("feature_lava_quest_un_lock", 999) }
        };
    }

    public static List<TypeFeature> GetUnlockedActiveFeatures()
    {
        int currentLevel = GameManager.Instance.GetMasterData().dataStage.Get();

        List<TypeFeature> unlockedFeatures = new List<TypeFeature>();

        foreach (var feature in FeatureUnlockLevels)
        {
            TypeFeature type = feature.Key;
            int unlockLevel = feature.Value;
            if (currentLevel == unlockLevel && FeatureActive.TryGetValue(type, out bool isActive) && isActive)
            {
                unlockedFeatures.Add(type);
            }
        }

        return unlockedFeatures;
    }

    public static bool ActiveEvent(TypeFeature type, int plusLevel = 0)
    {
        if (StaticDataFeature.FeatureActive.TryGetValue(type, out bool isActive) && !isActive)
        {
            return false;
        }
        else
        {
            if (StaticDataFeature.FeatureUnlockLevels.TryGetValue(type, out int levelUnlock))
            {
                int currentLevel = GameManager.Instance.GetMasterData().dataStage.Get();
                bool canShow = currentLevel >= (levelUnlock + plusLevel);
                return canShow;
            }
        }
        return false;
    }

    public static bool ActiveEventTSC()
    {
        if (ActiveEvent(TypeFeature.TheStickyCollector, 0))
        {
            if (GameManager.Instance.GetFeatureData().featureTSC.IsMax()) return false;
            return true;
        }
        return false;
    }
}
