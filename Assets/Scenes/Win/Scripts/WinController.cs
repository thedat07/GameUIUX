using Creator;
using AssetKits.ParticleImage;
using UnityEngine;
using DG.Tweening;

public partial class WinController : Controller
{
    public const string WIN_SCENE_NAME = "WinScene";

    public override string SceneName()
    {
        return WIN_SCENE_NAME;
    }

    WinData m_Data;

    public LuckySliderUI luckySliderUI;

    public ParticleImage particleFree;

    public ParticleImage particleAds;

    public CanvasGroup canvasGroupContainer;

    public Transform btnFree;

    public Transform btnAds;

    public Transform sliderTransform;

    public GameObject[] lstHide;

    private bool Unlock() => GameManager.Instance.GetMasterData().dataStage.Get() >= StaticData.LevelUnLockWin;

    void Awake()
    {
        Log();
        foreach (var item in lstHide)
        {
            item.SetActive(Unlock());
        }
    }

    void Log()
    {
        StaticData.IsRandomColor = false;
    }

    public void OnCoinAds()
    {
        GameManager.Instance.GetAdsModelView().ShowRewardedVideo("WinLucky", () =>
        {
            LogAds();
            canvasGroupContainer.blocksRaycasts = false;
            luckySliderUI.StopSpin((x) => { OnXCoin((int)x, StaticLogData.WinXCoin, particleAds, MasterModelView.TypeSource.Ads); });
        });
    }

    void LogAds()
    {
        string log = "Rewards" + "_" + "{0}";
        FirebaseEventLogger.LogMax(log);
    }

    public void OnCoin()
    {
        canvasGroupContainer.blocksRaycasts = false;
        luckySliderUI.StopSpin((x) => { OnXCoin(1, StaticLogData.WinCoin, particleFree, MasterModelView.TypeSource.Free); });
    }

    void OnXCoin(float x, string log, ParticleImage particle, MasterModelView.TypeSource typeSource)
    {
        canvasGroupContainer.blocksRaycasts = false;
        canvasGroupContainer.interactable = false;
        int coinX = (int)(StaticData.CoinWin * x);
        particle.rateOverLifetime = (coinX / 10) * 2;
        GameManager.Instance.GetMasterModelView().Post(coinX, MasterDataType.Money, log, typeSource);
        particle.Play();
        OnNext();
    }

    public void DoneEffect()
    {
        UnityTimer.Timer.Register(0.25f, () =>
        {
            btnFree.transform.DOScale(1.05f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            btnAds.transform.DOScale(1.05f, 0.5f).SetLoops(-1, LoopType.Yoyo).SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            Sequence seq = DOTween.Sequence();
            seq.Append(sliderTransform.DOShakePosition(
                    0.5f,
                    strength: new Vector3(10f, 0, 5f),
                    vibrato: 10));
            seq.AppendInterval(3f); // nghỉ 3s rồi lặp lại
            seq.SetLoops(-1);       // lặp vô hạn
            seq.SetLink(gameObject, LinkBehaviour.KillOnDestroy);
        }, autoDestroyOwner: this);
    }

    public void OnNext()
    {
        UnityTimer.Timer.Register(2.5f, () =>
            {
                if (StaticData.ShouldShowRate(GameManager.Instance.GetMasterData().dataStage.Get()))
                {
                    Creator.Director.PushScene(RateUsController.RATEUS_SCENE_NAME, onHidden: () =>
                    {
                        GameManager.Instance.PlayNext();
                    });
                }
                else
                {
                    GameManager.Instance.PlayNext();
                }
            }, autoDestroyOwner: this);

    }
}