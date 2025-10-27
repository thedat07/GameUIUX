using Creator;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using UniRx;
using TMPro;
using UnityEngine;
using Gley.EasyIAP.Internal;
using UnityUtilities;
using UnityTimer;

public class PiggyBankController : Controller
{
    public const string PIGGYBANK_SCENE_NAME = "PiggyBank";

    public override string SceneName()
    {
        return PIGGYBANK_SCENE_NAME;
    }

    [Header("View Full")]
    public GameObject objectFull;
    public TextMeshProUGUI expTextFull;

    [Header("View Not Full")]
    public GameObject objectNotFull;
    public Slider sliderNotFull;
    public TextMeshProUGUI expTextNotFull;

    [Header("View")]
    public TextMeshProUGUI textPrice;
    public TextMeshProUGUI[] textMoney;

    private FeaturePiggyBank m_Data;

    public GameObject btnGray;
    public GameObject btnBuy;
    public ButtonGameIAP btnIAP;

    public InventoryItem data;

    private Timer timerUpdate;

    void Start()
    {
        m_Data = GameManager.Instance.GetFeatureData().featurePiggyBank;

        m_Data.GetData().exp.Subscribe(newExp => ViewExp(newExp)).AddTo(this);

        m_Data.GetData().isFull.Subscribe(isFull => ViewFull(isFull)).AddTo(this);

        foreach (var item in textMoney)
        {
            item.text = StaticData.PiggyMoney.ToString();
        }

        if (!IAPManager.Instance.IsInitialized())
        {
            if (textPrice)
                textPrice.text = "???";
        }
        else
        {
            if (textPrice)
                textPrice.text = Gley.EasyIAP.API.GetLocalizedPriceString(Gley.EasyIAP.ShopProductNames.PiggyBank).ToString();
        }

        ViewExp(m_Data.GetData().exp.Value);
        ViewFull(m_Data.GetData().isFull.Value);
    }

    [Button]
    public void OnBuy()
    {
        if (m_Data.BuyPiggy(out int coin))
        {
            data.SetQuantity(coin);
            DataMethod r = new DataMethod(data, "Piggy Bank");
            r.Apply(typeSource: MasterModelView.TypeSource.Iap);
            Creator.ManagerDirector.PushScene(RewardsController.REWARDS_SCENE_NAME, r);
        }
        else
        {
            Console.Log("Không thể mua PiggyBank (chưa full hoặc đã hết hạn).");
        }
    }

    void ViewExp(int newExp)
    {
        int maxExp = m_Data.GetMaxExp();
        expTextNotFull.text = $"{newExp}/{maxExp}";
        sliderNotFull.FillAmount((float)newExp / maxExp);
    }

    void ViewFull(bool isFull)
    {
        objectFull.SetActive(isFull);
        objectNotFull.SetActive(!isFull);

        btnGray.SetActive(!isFull);
        btnBuy.SetActive(isFull);

        if (isFull)
        {
            if (sliderNotFull) sliderNotFull.value = 1f;

            if (timerUpdate != null) timerUpdate.Cancel();

            timerUpdate = Timer.Register(0.5f, ViewTime, isLooped: true, autoDestroyOwner: this);

            void ViewTime()
            {
                System.TimeSpan timerToShow = m_Data.GetRemainingTime();
                string formatD = "{0}d {1}h"; string formatH = "{0}h {1}m";
                string formatM = "{0}m {1}s"; string formatZero = "--";
                expTextFull.text = CountdownTextUtilities.FormatCountdownLives(timerToShow, formatD, formatH, formatM, formatZero);
            }
        }
        else
        {
            if (timerUpdate != null) timerUpdate.Cancel();
        }
    }
}