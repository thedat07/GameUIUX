using UnityEngine;
using UniRx;
using TMPro;
using UnityTimer;
using UnityEngine.UI;
using DG.Tweening;
using BrunoMikoski.AnimationSequencer;

public class ButtonHomeTSC : ButtonHomeFeature
{
    protected override TypeFeature GetTypeFeature()
    {
        return TypeFeature.TheStickyCollector;
    }

    protected override FeatureData GetData()
    {
        return GameManager.Instance.GetFeatureData().featureTSC;
    }

    protected FeatureTheStickyCollector GetDataTSC() => GetData() as FeatureTheStickyCollector;

    public SoGoal soStoreRewards;
    public SoImageItem soImage;
    public SoColorImage soColorImage;

    [SerializeField] GameObject m_View;
    [SerializeField] RectMask2D m_RectMask2DView;

    [Header("View Lock")]
    [SerializeField] GameObject m_Lock;
    [SerializeField] TextMeshProUGUI m_LockTile;

    [Header("View UnLock")]
    [SerializeField] GameObject m_UnLock;
    [SerializeField] TextMeshProUGUI m_TxtTime;
    [SerializeField] TextMeshProUGUI m_TxtInfo;
    [SerializeField] Slider m_Slider;

    [Header("View Icon")]
    [SerializeField] Transform m_PointIcon;
    [SerializeField] Image m_Icon;
    [SerializeField] SpineAnimUIUXController m_AnimIcon;

    [Header("View Cube")]
    [SerializeField] Image m_Cube;

    [Header("Anim")]
    public TextMeshProUGUI txtInfoVaule;
    public AnimationSequencerController anim;

    private FeatureTheStickyCollector m_Data => GameManager.Instance.GetFeatureData().featureTSC;
    private SoGoal.ProgressData m_ProgressData;

    protected override string GetLevelLock() => string.Format("Event unlock at <color=#747474>Level {0}</color>", StaticDataFeature.FeatureUnlockLevels[GetTypeFeature()]);

    public override GameObject GetContent()
    {
        return m_View;
    }

    protected override void ActiveObject(bool active)
    {
        Vector4 padding = m_RectMask2DView.padding;
        padding.w = active ? 250 : 0;

        Vector2Int soft = m_RectMask2DView.softness;
        soft.y = active ? 300 : 20;

        m_RectMask2DView.padding = padding;
        m_RectMask2DView.softness = soft;

        base.ActiveObject(active);
    }

    protected override void ViewLock()
    {
        m_LockTile.text = GetLevelLock();
        m_Slider.value = 0;
        m_Lock.SetActive(true);
        m_UnLock.SetActive(false);
    }

    void UpdateView()
    {
        ViewSlider();
        ViewTime();
        if (GameManager.Instance.GetFeatureData().featureTSC.IsMax())
            ActiveObject(false);
    }

    protected override void ViewUnlockLock()
    {
        UpdateView();

        m_View.SetActive(true);
        m_Lock.SetActive(false);
        m_UnLock.SetActive(true);

        Timer.Register(0.5f, () =>
        {
            ViewTime();
        }, isLooped: true, autoDestroyOwner: this);

        AutoClaim();

        GetDataTSC().GetData().claimIndex.Subscribe(_ => { UpdateView(); }).AddTo(this);
    }

    protected override void Click()
    {
        HomeController.Instance.OnTSC();
    }

    void ViewTime()
    {
        m_TxtTime.text = m_Data.GetTime();
    }

    void ViewSlider()
    {
        int amount = GetDataTSC().GetData().amount.Value;
        m_ProgressData = soStoreRewards.GetProgress(amount);
        m_TxtInfo.text = m_ProgressData.ToString();

        UpdateViewIcon(GetDataTSC().GetData().claimIndex.Value);

        ShowIcon(m_ProgressData.index);

        m_Cube.sprite = soColorImage.GetImage(GetDataTSC().GetColor());
        txtInfoVaule.gameObject.SetActive(false);

        if (GameManager.Instance.GetFeatureModelView().dataWin.TryGetValue(TypeFeature.TheStickyCollector, out int value))
        {
            txtInfoVaule.gameObject.SetActive(true);
            txtInfoVaule.text = string.Format("+{0}", value);
            anim.OnFinishedEvent.AddListener(() =>
            {
                txtInfoVaule.gameObject.SetActive(false);
            });
            m_Slider.DOValue(m_ProgressData.GetProgress(), 0.5f).SetDelay(0.5f).SetLink(gameObject, LinkBehaviour.KillOnDestroy);
            anim.Play();
        }
        else
        {
            m_Slider.value = m_ProgressData.GetProgress();
        }
    }

    void ShowIcon(int index)
    {
        m_PointIcon.DOKill();
        m_PointIcon.DOScale(Vector3.one, 0.25f).From(0).SetLink(gameObject, LinkBehaviour.KillOnDestroy).SetEase(Ease.OutBack);

        bool r = soStoreRewards.datas[index].rewards.Length > 1;
        m_Icon.gameObject.SetActive(!r);
        m_AnimIcon.gameObject.SetActive(r);

        if (r)
        {
            int skin = soStoreRewards.datas[index].indexIcon;
            m_AnimIcon.GetSkeletonGraphic().Skeleton.SetSkin(skin.ToString());
            m_AnimIcon.PlayIdleOnly();
        }
        else
        {
            Sprite icon =  soImage.GetImage(soStoreRewards.datas[index].rewards[0].GetDataType());
            m_Icon.sprite = icon;
        }
    }

    void AutoClaim()
    {
        int indexClaim = m_ProgressData.GetIndexClaim();
        int curIndex = GetDataTSC().GetData().claimIndex.Value;
        int countLoop = 0;

        if (curIndex < indexClaim)
        {
            int loopCount = indexClaim - curIndex;
            int baseIndex = curIndex;

            m_Slider.DOKill();

            if (loopCount > 0)
            {
                m_Slider
               .DOValue(1, 0.5f) // thời gian mỗi vòng, có thể chỉnh theo ý
               .From(0)
               .SetLoops(loopCount, LoopType.Restart)
               .SetLink(gameObject, LinkBehaviour.KillOnDestroy)
               .OnStepComplete(() =>
               {
                   countLoop++;
                   int updateIndex = baseIndex + countLoop;
                   UpdateViewIcon(updateIndex);
                   Console.Log($"Loop {countLoop}/{loopCount} → UpdateViewIcon({updateIndex})");
               })
               .OnComplete(
                () =>
                    {
                        m_Slider.DOValue(m_ProgressData.GetProgress(), 0.5f).From(0).OnComplete(OnRewards).SetLink(gameObject, LinkBehaviour.KillOnDestroy);
                    }
               );
            }
            else
            {
                OnRewards();
            }

            void OnRewards()
            {
                GetDataTSC().Claim(indexClaim);

                UpdateViewIcon(indexClaim);

                var dataRewards = soStoreRewards.GetData(curIndex, GetDataTSC().GetData().claimIndex.Value);

                DataMethod r = new DataMethod(
                    HelperCreator.Convert(dataRewards),
                    "Arcane Cube",
                    StaticLogData.LogArcaneRewards(indexClaim + 1)
                );
                r.Apply();

                Creator.ManagerDirector.PushScene(RewardsController.REWARDS_SCENE_NAME, r);
            }
        }
    }

    void UpdateViewIcon(int index)
    {
        if (soStoreRewards == null || soStoreRewards.datas == null || index < 0 || index >= soStoreRewards.datas.Length)
        {
            return;
        }

        var data = soStoreRewards.datas[index];
        
        if (data == null || data.rewards == null || data.rewards.Length == 0)
        {
            return;
        }

        ShowIcon(index);
    }
}
