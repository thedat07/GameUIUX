using UnityEngine;
using UnityEngine.Events;
using ExaGames.Common.TimeBasedLifeSystem;

public enum GameColor
{
    Blue = 0,
    Green = 1,
    Red = 2,
    Yellow = 3,
    Orange = 4,
    Pink = 5,
    Purple = 6,
    DarkBlue = 7,
    DarkGreen = 8,
    LightBlue = 9,

    StarBlue = 10,
    StarGreen = 11,
    StarRed = 12,
    StarYellow = 13,
    StarOrange = 14,
    StarPink = 15,
    StarPurple = 16,
    StarDarkBlue = 17,
    StarDarkGreen = 18,
    StarLightBlue = 19,
}
public class MasterModelView : MonoBehaviour, IInitializable
{
    [System.Serializable]
    public class InfoGamePlay
    {
        public int countPlay;
        public int countWin;
        public int countLose;
        public int winStreak;
        public int loseStreak;

        public InfoGamePlay()
        {
            countWin = 0;
            countLose = 0;
            winStreak = 0;
            loseStreak = 0;
        }

        public void Win()
        {
            loseStreak = 0;
            countWin += 1;
            winStreak += 1;
        }

        public void Lose()
        {
            winStreak = 0;
            countLose += 1;
            loseStreak += 1;
        }
    }

    public enum TypeSource
    {
        Free,
        Ads,
        Iap,
        Coin
    }

    private MasterModel m_Model;

    public LivesManager livesManager;

    public LivesManager x2ItemManager;

    public GameObject debugObject;

    public InfoGamePlay infoGame;

    public bool IsTest = false;

    public bool IsPlay = false;

    public bool CanPlay() => livesManager.CanPlay;

    public bool IsX2Item() => x2ItemManager.HasInfiniteLives;

    public void ConsumeLife() => livesManager.ConsumeLife();

    public void Initialize()
    {
        infoGame = new InfoGamePlay();
        IsTest = false;
        m_Model = GameManager.Instance.GetMasterData();
        StaticData.SeassonGame += 1;
    }

    public void ActiveTest()
    {
        IsTest = true;
        debugObject.SetActive(IsTest);
    }

    public void Post(int vaule, MasterDataType type, string log = "", TypeSource typeSource = TypeSource.Free)
    {
        switch (type)
        {
            case MasterDataType.Stage:
                {
                    m_Model.dataStage.Post(vaule);
                }
                break;
            case MasterDataType.Money:
                {
                    int remainingValue = m_Model.dataMoney.Get();
                    m_Model.dataMoney.Post(vaule);
                    int totalEarnedValue = m_Model.dataMoney.Get();
                    Log(remainingValue, totalEarnedValue, "Currency");

                    FirebaseUserProperties.SetRemainingGold(m_Model.dataMoney.Get());
                    if (vaule > 0)
                    {
                        StaticData.TotalGoldEarn += vaule;
                        FirebaseUserProperties.SetTotalGoldEarn(StaticData.TotalGoldEarn);
                    }
                }
                break;
            case MasterDataType.Booster1:
                {
                    int remainingValue = m_Model.dataBooster1.Get();
                    m_Model.dataBooster1.Post(vaule);
                    int totalEarnedValue = m_Model.dataBooster1.Get();
                    Log(remainingValue, totalEarnedValue, "Resource");
                }
                break;
            case MasterDataType.Booster2:
                {
                    int remainingValue = m_Model.dataBooster2.Get();
                    m_Model.dataBooster2.Post(vaule);
                    int totalEarnedValue = m_Model.dataBooster2.Get();
                    Log(remainingValue, totalEarnedValue, "Resource");
                }
                break;
            case MasterDataType.Booster3:
                {
                    int remainingValue = m_Model.dataBooster3.Get();
                    m_Model.dataBooster3.Post(vaule);
                    int totalEarnedValue = m_Model.dataBooster3.Get();
                    Log(remainingValue, totalEarnedValue, "Resource");
                }
                break;
            case MasterDataType.LivesInfinity:
                {
                    livesManager.GiveInifinite(vaule);
                    if (vaule > 0)
                        GameManager.Instance.GetSettingModelView().PlaySound(TypeAudio.Life, 0.25f);
                }
                break;
            case MasterDataType.X2Item:
                {
                    x2ItemManager.GiveInifinite(vaule);
                }
                break;
            case MasterDataType.Lives:
                {
                    int remainingValue = livesManager.Lives;

                    if (vaule < 5)
                    {
                        for (int i = 0; i < vaule; i++)
                        {
                            livesManager.GiveOneLife();
                        }
                    }
                    else
                    {
                        livesManager.FillLives();
                    }

                    if (vaule > 0)
                    {
                        GameManager.Instance.GetSettingModelView().PlaySound(TypeAudio.Life, 0.25f);
                    }

                    int totalEarnedValue = livesManager.Lives;
                    Log(remainingValue, totalEarnedValue, "Resource");
                }
                break;
            default:
                break;
        }

        void Log(int remainingValue, int totalEarnedValue, string category)
        {
            if (log != "")
            {
                int earn_vaule = vaule;
                int remaining_value = remainingValue;
                int total_earned_value = totalEarnedValue;

                string itemCategory = category;
                string itemId = type.ToString();
                string source = typeSource.ToString();
                string sourceId = log;

                if (vaule > 0)
                {
                    if (typeSource == TypeSource.Iap)
                        FirebaseEventLogger.LogBuyResource(itemCategory, itemId, source, sourceId, earn_vaule, remaining_value, total_earned_value);
                    else
                        FirebaseEventLogger.LogEarnResource(itemCategory, itemId, source, sourceId, earn_vaule, remaining_value, total_earned_value);
                }
                else
                {
                    FirebaseEventLogger.LogSpendResource(itemCategory, itemId, sourceId, source, sourceId, earn_vaule, remaining_value, total_earned_value);
                }
            }
        }

    }

    public void Put(int vaule, MasterDataType type)
    {
        switch (type)
        {
            case MasterDataType.Stage:
                {
                    m_Model.dataStage.Put(vaule);
                }
                break;
        }
    }

    public void PostMoney(int vaule, string log, UnityAction onSucccess, UnityAction onFail, UnityAction onCompleted)
    {
        int newMoney = m_Model.GetData(MasterDataType.Money) - vaule;
        if (newMoney < 0)
        {
            onFail?.Invoke();
            FirebaseEvent.LogEvent(StaticLogData.LogMoney[0],
            StaticLogData.LogInfoFirebase[0], GameManager.Instance.GetMasterData().GetData(MasterDataType.Stage).ToString(),
            StaticLogData.LogInfoFirebase[1], vaule.ToString(),
            StaticLogData.LogInfoFirebase[2], log);
        }
        else
        {
            onSucccess?.Invoke();
            Post(-vaule, MasterDataType.Money);
            FirebaseEvent.LogEvent(StaticLogData.LogMoney[1],
            StaticLogData.LogInfoFirebase[0], GameManager.Instance.GetMasterData().GetData(MasterDataType.Stage).ToString(),
            StaticLogData.LogInfoFirebase[1], vaule.ToString(),
            StaticLogData.LogInfoFirebase[2], log);
        }
        onCompleted?.Invoke();
    }
}
