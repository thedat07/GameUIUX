using Creator;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyBoosterData
{
    public MasterDataType type;

    public BuyBoosterData(MasterDataType type)
    {
        this.type = type;
    }
}

public class BuyBoosterController : Controller
{
    public const string BUYBOOSTER_SCENE_NAME = "BuyBooster";

    public override string SceneName()
    {
        return BUYBOOSTER_SCENE_NAME;
    }

    public UnLockSO unLockSO;

    [SerializeField] TextMeshProUGUI m_TxtName;
    [SerializeField] TextMeshProUGUI m_TxtDes;
    [SerializeField] TextMeshProUGUI m_TxtPrice;
    [SerializeField] Image m_Icon;
    BuyBoosterData m_Data;

    int[] m_DataPrices = new int[3] { StaticData.CoinBooster1, StaticData.CoinBooster2, StaticData.CoinBooster3 };

    private UnLockSO.Data m_DataType;

    private int GetIndex()
    {
        switch (m_Data.type)
        {
            case MasterDataType.Booster1:
                return 0;
            case MasterDataType.Booster2:
                return 1;
            case MasterDataType.Booster3:
                return 2;
        }
        return 0;
    }

    private int GetCoin() => m_DataPrices[GetIndex()];

    public override void OnActive(object data)
    {
        if (data != null)
        {
            m_Data = data as BuyBoosterData;
            m_DataType = unLockSO.GetData(m_Data.type);
            m_TxtName.text = m_DataType.txtTile;
            m_TxtDes.text = m_DataType.txtTut;
            m_Icon.sprite = m_DataType.icon;
        }
        m_TxtPrice.text = GetCoin().ToString();
    }

    public void OnWatchAds()
    {
        GameManager.Instance.GetAdsModelView().ShowRewardedVideo(StaticLogData.BuyBooster, () =>
        {
            OnBuy(MasterModelView.TypeSource.Ads);
            LogAds();
        });
    }

    void LogAds()
    {
        FirebaseEvent.LogEventReward(m_Data.type.ToString());
        string log = m_Data.type.ToString() + "_" + "{0}";
        FirebaseEventLogger.LogMax(log);
    }

    public void OnCoin()
    {
        GameManager.Instance.GetMasterModelView().PostMoney(GetCoin(), StaticLogData.BuyBooster, () =>
        {
            OnBuy(MasterModelView.TypeSource.Coin);
        }, () =>
        {
            ManagerDirector.PushScene(ShopMiniController.SHOPMINI_SCENE_NAME);
        }, null);
    }

    void OnBuy(MasterModelView.TypeSource typeSource)
    {
        GameManager.Instance.GetMasterModelView().Post(3, m_Data.type, StaticLogData.BuyBooster, typeSource);
        OnKeyBack();
    }
}