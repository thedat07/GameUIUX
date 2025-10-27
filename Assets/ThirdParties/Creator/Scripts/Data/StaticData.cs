using UnityEngine;
using UnityUtilities;

public static class StaticData
{
    public static string WebSup = "";

    public static Vector2 ScreenGame = new Vector2(1080, 2340);

    public static float DelayTimeDefault = 0.2f;

    private const string Key = "player_id";

    public static string GetPlayerId()
    {
        if (!PlayerPrefs.HasKey(Key))
        {
            string newId = System.Guid.NewGuid().ToString();
            PlayerPrefs.SetString(Key, newId);
            PlayerPrefs.Save();
        }
        return PlayerPrefs.GetString(Key);
    }

    public static string GetPlayerIdSubstring()
    {
        string fullId = GetPlayerId();

        return fullId.Length > 10
            ? fullId.Substring(fullId.Length - 10)
            : fullId;
    }

    public static bool IsRandomColor
    {
        get
        {
            if (RandomColor)
            {
                return PlayerPrefs.GetInt("GamePlayRandomColor", 0) == 1;
            }
            else
            {
                return false;
            }
        }
        set
        {
            PlayerPrefs.SetInt("GamePlayRandomColor", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static int IAPCount
    {
        get
        {
            return PlayerPrefs.GetInt("IAPCount", 0);
        }
        set
        {
            PlayerPrefs.SetInt("IAPCount", value);
            PlayerPrefs.Save();
        }
    }

    public static int TotalGoldEarn
    {
        get
        {
            return PlayerPrefs.GetInt("TotalGoldEarn", 0);
        }
        set
        {
            PlayerPrefs.SetInt("TotalGoldEarn", value);
            PlayerPrefs.Save();
        }
    }

    public static int SeassonGame
    {
        get
        {
            return PlayerPrefs.GetInt("SeassonGame", 0);
        }
        set
        {
            PlayerPrefs.SetInt("SeassonGame", value);
            PlayerPrefs.Save();
        }
    }

    public static bool IsUnLocNewFeature
    {
        get
        {
            return PlayerPrefs.GetInt("IsUnLocNewFeature", 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("IsUnLocNewFeature", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static bool NotiProfile
    {
        get
        {
            return PlayerPrefs.GetInt("NotiProfile", 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("NotiProfile", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static bool RateUs
    {
        get
        {
            return PlayerPrefs.GetInt("RateUs", 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("RateUs", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static bool ShouldShowRate(int currentLevel)
    {
        string LAST_SHOW_LEVEL = "LastShowRateLevel";

        // Nếu user đã rate thì không show nữa
        if (StaticData.RateUs)
            return false;

        int lastShowLevel = PlayerPrefs.GetInt(LAST_SHOW_LEVEL, 0);

        // Lần đầu ở level 6
        if (currentLevel == 7)
        {
            PlayerPrefs.SetInt(LAST_SHOW_LEVEL, currentLevel);
            PlayerPrefs.Save();
            return true;
        }

        // Sau đó mỗi 10 level thì show 1 lần
        if (currentLevel - lastShowLevel >= 10)
        {
            PlayerPrefs.SetInt(LAST_SHOW_LEVEL, currentLevel);
            PlayerPrefs.Save();
            return true;
        }

        return false;
    }

    public static bool IsFirstOpenTSC
    {
        get
        {
            return PlayerPrefs.GetInt("IsFirstOpenTSC", 0) == 1;
        }
        set
        {
            PlayerPrefs.SetInt("IsFirstOpenTSC", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static bool JoinFacebook
    {
        get => PlayerPrefs.GetInt("JoinedFacebook", 0) == 1;
        set
        {
            PlayerPrefs.SetInt("JoinedFacebook", value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }

    public static int TimeRevive
    {
        get
        {
            return RemoteConfigController.GetIntConfig("time_revive", 20);
        }
    }

    public static int TimeFrezze
    {
        get
        {
            return RemoteConfigController.GetIntConfig("time_frezze", 10);
        }
    }

    public static int InterTimestep
    {
        get
        {
            return RemoteConfigController.GetIntConfig("inter_capping", 180);
        }
    }

    public static int MinGameTimeForInter
    {
        get
        {
            if (!GameManager.Instance.GetMasterModelView().IsTest)
                return RemoteConfigController.GetIntConfig("mingame_time_for_inter", 600);
            else
                return 1;
        }
    }


    public static int InterTimestepRw
    {
        get
        {
            return RemoteConfigController.GetIntConfig("inter_capping_rw", 180);
        }
    }

    public static int LevelStartShowingInter
    {
        get
        {
            return RemoteConfigController.GetIntConfig("inter_start_level", 10);
        }
    }

    public static float RateRev
    {
        get
        {
            return RemoteConfigController.GetFloatConfig("af_purchase_manual", 0.7f);
        }
    }

    public static int MaxAdsSpins
    {
        get
        {
            return RemoteConfigController.GetIntConfig("max_ads_spins", 2);
        }
    }

    public static int TimeCooldownHoursDaily
    {
        get
        {
            return RemoteConfigController.GetIntConfig("time_cooldown_hours_daily", 1);
        }
    }

    public static int LevelBackHome
    {
        get
        {
            return RemoteConfigController.GetIntConfig("level_back_home", 15);
        }
    }

    public static int LevelUnLockWin
    {
        get
        {
            return RemoteConfigController.GetIntConfig("level_unlock_win", 15);
        }
    }

    public static int CoinKeepPlaying
    {
        get
        {
            return RemoteConfigController.GetIntConfig("coin_keep_playing", 900);
        }
    }

    public static int CoinRefil
    {
        get
        {
            return RemoteConfigController.GetIntConfig("coin_refill", 900);
        }
    }

    public static int CoinWin
    {
        get
        {
            return RemoteConfigController.GetIntConfig("coin_win", 40);
        }
    }

    public static int CoinBooster1
    {
        get
        {
            return RemoteConfigController.GetIntConfig("coin_booster_1", 900);
        }
    }

    public static int CoinBooster2
    {
        get
        {
            return RemoteConfigController.GetIntConfig("coin_booster_2", 1200);
        }
    }

    public static int CoinBooster3
    {
        get
        {
            return RemoteConfigController.GetIntConfig("coin_booster_3", 1600);
        }
    }

    public static bool RandomColor
    {
        get
        {
            return RemoteConfigController.GetBoolConfig("random_color", true);
        }
    }

    public static int PiggyMoney
    {
        get
        {
            return RemoteConfigController.GetIntConfig("feature_piggy_money", 2000);
        }
    }

    public static int LavaQuestMoney
    {
        get
        {
            return RemoteConfigController.GetIntConfig("feature_lava_quest_rewards", 100000);
        }
    }
}
