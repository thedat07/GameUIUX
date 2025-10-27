using Creator;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;
using System.Collections.Generic;

public class TheStickyCollectorController : Controller
{
    public const string THESTICKYCOLLECTOR_SCENE_NAME = "TheStickyCollector";

    public override string SceneName()
    {
        return THESTICKYCOLLECTOR_SCENE_NAME;
    }

    public FeatureTheStickyCollector GetDataTSC() => GameManager.Instance.GetFeatureData().featureTSC;

    public SoGoal soStoreRewards;
    public SoColorImage soColorImage;

    public GameObject view;
    public GameObject viewR;
    public Transform point;

    [Header("View")]
    public TextMeshProUGUI txtInfo;
    public TextMeshProUGUI txtIndex;
    public Slider sliderInfo;
    public TextMeshProUGUI[] txtTime;
    public Image icon;

    [Header("Scroll")]
    public RectTransform content;  // Content của ScrollRect
    public float padding;
    private SoGoal.ProgressData m_ProgressData;

    [Header("Content")]
    public GameObject Info;
    public GameObject Tut;
    public GameObject Content;

    [Header("View")]
    public InfoItemsView infoItemsView;
    public Transform contentInfo;

    public SpineAnimUIUXController m_AnimInfo;
    public SpineAnimUIUXController m_AnimContent;
    List<ItemTSCView> itemTSCViews;

    void Start()
    {
        itemTSCViews = new List<ItemTSCView>();
        ViewObject();
        InitData();
        int index = soStoreRewards.datas.Length - 1;
        for (int i = soStoreRewards.datas.Length - 1; i >= 0; i--)
        {
            bool r = soStoreRewards.datas[i].rewards.Length > 1;
            GameObject spawn = r ? viewR : view;
            var objectSpawn = Instantiate(spawn, point);
            if (objectSpawn.transform.GetChild(1).TryGetComponent<ItemTSCView>(out ItemTSCView itemTSC))
            {
                itemTSC.Init(index, soStoreRewards, m_ProgressData, this);
                index -= 1;
                itemTSCViews.Add(itemTSC);
            }
        }

        View();

        UnityTimer.Timer.Register(0.5f, () =>
        {
            foreach (var item in txtTime)
            {
                item.text = GetDataTSC().GetTime();
            }
        }, isLooped: true, autoDestroyOwner: this);

        GetDataTSC().GetData().amount.Subscribe(_ => { UpdateView(); }).AddTo(this);

        m_AnimInfo.PlayActionIdleLoop();
        m_AnimContent.PlayActionIdleLoop();
    }

    void ViewObject()
    {
        Info.SetActive(!StaticData.IsFirstOpenTSC);
        Content.SetActive(StaticData.IsFirstOpenTSC);
        Tut.SetActive(false);
        ActiveFirst();

        void ActiveFirst()
        {
            if (!StaticData.IsFirstOpenTSC)
            {
                StaticData.IsFirstOpenTSC = true;
            }
        }
    }

    void UpdateView()
    {
        InitData();
        View();
        foreach (var item in itemTSCViews)
        {
            item.UpdateView(m_ProgressData);
        }
    }

    void InitData()
    {
        int amount = GetDataTSC().GetData().amount.Value;
        m_ProgressData = soStoreRewards.GetProgress(amount);
    }

    void View()
    {
        txtInfo.text = m_ProgressData.ToString();
        txtIndex.text = m_ProgressData.GetIndex();
        sliderInfo.DOValue(m_ProgressData.GetProgress(), 0.25f).SetLink(gameObject, LinkBehaviour.KillOnDestroy).SetEase(Ease.Linear);
        if (m_ProgressData.index + 1 >= 5)
            ScrollTo((m_ProgressData.index - 5) + 1);
        icon.sprite = soColorImage.GetImage(GetDataTSC().GetColor());
        foreach (var item in txtTime)
        {
            item.text = GetDataTSC().GetTime();
        }
    }

    void ScrollTo(int index)
    {
        Vector2 archor = content.anchoredPosition;
        archor.y += (padding * index * -1);
        content.anchoredPosition = archor;
    }

    public GameObject SpawnViewInfo(InventoryItem[] data, RectTransform point)
    {
        GameObject spawn;
        spawn = Lean.Pool.LeanPool.Spawn(infoItemsView.gameObject, contentInfo);
        if (spawn.TryGetComponent<InfoItemsView>(out InfoItemsView infoItems))
        {
            infoItems.Init(data, point);
        }
        return spawn;
    }
}