using Creator;
using TMPro;
using UnityEngine;
using ExaGames.Common.TimeBasedLifeSystem;
using UnityEngine.Events;

public class MoreLivesData
{
    public UnityAction onX;

    public UnityAction onBuy;

    public bool activeCoin;

    public MoreLivesData(UnityAction onX, UnityAction onBuy, bool activeCoin = true)
    {
        this.onX = onX;
        this.onBuy = onBuy;
        this.activeCoin = activeCoin;
    }
}

public class MoreLivesController : Controller
{
    public const string MORELIVES_SCENE_NAME = "MoreLives";

    LivesManager GetLives() => GameManager.Instance.GetMasterModelView().livesManager;

    public override string SceneName()
    {
        return MORELIVES_SCENE_NAME;
    }

    private MoreLivesData m_Data;

    [SerializeField] GameObject m_Coin;

    [SerializeField] GameObject m_PackCoin;

    public override void OnActive(object data)
    {
        if (data != null)
        {
            m_Data = data as MoreLivesData;
            m_Coin.SetActive(m_Data.activeCoin);
            //  m_PackCoin.SetActive(m_Data.activeCoin);
        }
        m_TxtPrice.text = StaticData.CoinRefil.ToString();
        View();
        UnityTimer.Timer.Register(0.5f, View, isLooped: true, autoDestroyOwner: this);
    }

    public void OnWatchAds()
    {
        GameManager.Instance.GetAdsModelView().ShowRewardedVideo(StaticLogData.MoreLives, () =>
        {
            string log = "MoreLives" + "_" + "{0}";
            FirebaseEventLogger.LogMax(log);

            GameManager.Instance.GetMasterModelView().Post(1, MasterDataType.Lives, StaticLogData.MoreLives, MasterModelView.TypeSource.Ads);

            if (m_Data != null)
            {
                m_Data?.onBuy?.Invoke();
            }
            else
            {
                if (!StaticDataFeature.ActiveEventTSC())
                    ManagerDirector.ReplaceScene(RetryController.RETRY_SCENE_NAME, new RetryData("Failed!", "Try Again", 0, OnKeyBack, () => { }));
                else
                    ManagerDirector.ReplaceScene(RetryController.RETRY_SCENE_NAME, new RetryData("You wil faill the Arcane Cube!", "Try Again", 3, OnKeyBack, () => { }));
            }
        });
    }

    public void OnClose()
    {
        if (m_Data != null)
        {
            m_Data?.onX?.Invoke();
        }
        else
        {
            OnKeyBack();
        }
    }

    public void OnCoin()
    {
        GameManager.Instance.GetMasterModelView().PostMoney(StaticData.CoinRefil, StaticLogData.MoreLives, () =>
        {
            GameManager.Instance.GetMasterModelView().Post(5, MasterDataType.Lives, StaticLogData.MoreLives, MasterModelView.TypeSource.Coin);

            if (m_Data != null)
            {
                m_Data?.onBuy?.Invoke();
            }
            else
            {
                if (!StaticDataFeature.ActiveEventTSC())
                    ManagerDirector.ReplaceScene(RetryController.RETRY_SCENE_NAME, new RetryData("Failed!", "Try Again", 0, OnKeyBack, () => { }));
                else
                    ManagerDirector.ReplaceScene(RetryController.RETRY_SCENE_NAME, new RetryData("You wil faill the Arcane Cube!", "Try Again", 3, OnKeyBack, () => { }));
            }

        }, () =>
        {
            ManagerDirector.ReplaceScene(ShopMiniController.SHOPMINI_SCENE_NAME);
        }, null);
    }

    [SerializeField] TextMeshProUGUI m_TxtInfo;

    [SerializeField] TextMeshProUGUI m_TxtLive;

    [SerializeField] TextMeshProUGUI m_TxtPrice;

    void View()
    {
        m_TxtInfo.text = string.Format("Next heart Time: <size=115%>{0}", GetLives().RemainingTimeString);
        m_TxtLive.text = string.Format("{0}", GetLives().Lives);
    }
}