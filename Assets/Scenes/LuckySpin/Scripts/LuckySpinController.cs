using Creator;
using TMPro;
using UnityEngine;
using UniRx;
using UnityTimer;

public class LuckySpinController : Controller
{
    public const string LUCKYSPIN_SCENE_NAME = "LuckySpin";

    public override string SceneName()
    {
        return LUCKYSPIN_SCENE_NAME;
    }

    [Header("View")]

    public TextMeshProUGUI infoText;

    private FeatureLukySpin m_Data;

    public GameObject buttonFree;

    public GameObject buttonAds;

    public RewardsItemView[] lstItem;

    [SerializeField] SpinWheelController m_SpinWheelController;

    [Header("So")]
    public SoStoreRewards soStoreRewards;

    private Timer countdownTimer;

    void Start()
    {
        m_Data = GameManager.Instance.GetFeatureData().featureLukySpin;

        // Lắng nghe freeSpinUsed thay đổi
        m_Data.GetData().freeSpinUsed
            .Subscribe(_ => UpdateInfoText())
            .AddTo(this);

        // Lắng nghe adsSpinsUsed thay đổi
        m_Data.GetData().adsSpinsUsed
            .Subscribe(_ => UpdateInfoText())
            .AddTo(this);

        // Lắng nghe reset ngày (lastSpinDate)
        m_Data.GetData().lastSpinDate
            .Subscribe(_ => UpdateInfoText())
            .AddTo(this);

        // Khởi tạo text ban đầu
        UpdateInfoText();

        m_SpinWheelController.Init(m_Data);

        ViewReawrds();
    }

    void ViewReawrds()
    {
        for (int i = 0; i < soStoreRewards.datas.Length; i++)
        {
            var item = lstItem[i];
            var data = soStoreRewards.datas[i];
            item.Init(data.rewards[0], data.rewards[0].GetIcon());
        }
    }

    private void UpdateInfoText()
    {
        var (freeLeft, adsLeft) = m_Data.GetRemainingSpins();

        if (infoText)
        {
            if (freeLeft > 0 || adsLeft > 0)
            {
                StopCountdown();

                infoText.text = string.Format("Daily: <color=#8AFA96>{0}/{1}</color>",
                    freeLeft + adsLeft,
                    StaticData.MaxAdsSpins + 1); // +1 nếu tính cả free
            }
            else
            {
                // Hết lượt -> show time
                infoText.text = m_Data.GetTime();
                StartCountdown();
            }
        }

        if (freeLeft > 0)
        {
            // Ưu tiên free
            if (buttonFree) buttonFree.SetActive(true);
            if (buttonAds) buttonAds.SetActive(false);
        }
        else
        {
            // Hết free -> cho dùng ads nếu còn
            if (buttonFree) buttonFree.SetActive(false);
            if (buttonAds) buttonAds.SetActive(adsLeft > 0);
        }

        m_Data.GetTime();
    }

    // Nút spin thường
    public void OnFreeSpin()
    {
        if (m_Data.CanSpin(false))
        {
            m_Data.DoSpin(false);
            m_SpinWheelController.StartSpin(MasterModelView.TypeSource.Free);
            Console.Log("Free spin thành công!");
        }
        else
        {
            Console.Log("Free spin đã hết!");
        }
    }

    // Nút spin Ads
    public void OnAdsSpin()
    {
        if (m_Data.CanSpin(true))
        {
            GameManager.Instance.GetAdsModelView().ShowRewardedVideo("LuckySpin", () =>
            {
                m_Data.DoSpin(true);
                m_SpinWheelController.StartSpin(MasterModelView.TypeSource.Ads);
                Console.Log("Ads spin thành công!");
            });
        }
        else
        {
            Console.Log("Ads spin đã hết!");
        }
    }

    private void StartCountdown()
    {
        StopCountdown();

        // Loop update text mỗi giây
        countdownTimer = Timer.Register(1f, () =>
        {
            if (infoText)
                infoText.text = m_Data.GetTime();
        }, isLooped: true, autoDestroyOwner: this);
    }

    private void StopCountdown()
    {
        if (countdownTimer != null)
        {
            countdownTimer.Cancel();
            countdownTimer = null;
        }
    }

    private void OnDisable()
    {
        StopCountdown();
    }
}