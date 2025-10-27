using UnityEngine;

public static class StaticLogData
{
    public static string MoreLives = "more_lives";

    public static string WinXCoin = "win_x_coin";

    public static string WinCoin = "win_coin";

    public static string KeepPlaying = "keep_playing";

    public static string BuyBooster = "buy_booster";

    public static string JoinFacebook = "join_facebook";

    public static string DataInGame = "rw_data_game";

    public static string LogPack(string packName) => string.Format("buy_pack_{0}", packName.ToLower());

    public static string LogTutorial(string type) => string.Format("tutorial_{0}", type.ToLower());

    public static string LogDailyRewards(string type) => string.Format("daily_rewards_{0}", type.ToLower());

    public static string LogSpinRewards(int index) => string.Format("spin_rewards_{0}", index.ToString());

    public static string LogArcaneRewards(int index) => string.Format("arcane_rewards_{0}", index.ToString());

    public static string[] LogMoney = new string[] { "money_spend_fail", "money_spend_success" };

    public static string[] LogInfoFirebase = new string[] { "level", "vaule", "log", "type_data" };
}
