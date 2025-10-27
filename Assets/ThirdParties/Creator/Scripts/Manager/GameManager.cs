using UnityEngine;
using DesignPatterns;
using Bytenado;

public partial class GameManager : SingletonPersistent<GameManager>
{
    public bool hideUI;

    [SerializeField] SettingModel m_SettingModel;
    [SerializeField] MasterModel m_MasterModel;
    [SerializeField] FeatureModel m_FeatureModel;
    [SerializeField] AdsModel m_AdsModel;


    [Header("ModelView")]
    [SerializeField] SettingModelView m_SettingModelView;
    [SerializeField] MasterModelView m_MasterModelView;
    [SerializeField] StageModelView m_StageModelView;
    [SerializeField] FeatureModelView m_FeatureModelView;
    [SerializeField] AdsModelView m_AdsModelView;
    [SerializeField] ShopModelView m_ShopModelView;
    [SerializeField] FirebaseController m_ConfigController;
    [SerializeField] FacebookController m_FacebookController;
    [SerializeField] NetworkTimeManager m_NetworkTimeManager;

    public bool IsDoneFirebase() => m_ConfigController.IsDone();
    public bool IsDoneFacebook() => m_FacebookController.IsDone();

    public override void Awake()
    {
        Init();
    }

    public void Init()
    {
        RemoteConfigController.LoadCacheConfig();
        
        m_NetworkTimeManager.Init();
        m_ConfigController.Initialize();
        m_FacebookController.Initialize();

        m_MasterModel = new MasterModel();
        m_SettingModel = new SettingModel();
        m_FeatureModel = new FeatureModel();
        m_AdsModel = new AdsModel();

        m_SettingModelView.Initialize();
        m_MasterModelView.Initialize();
        m_StageModelView.Initialize();
        m_AdsModelView.Initialize();
        m_ShopModelView.Initialize();
    }

    void Start()
    {
        m_FeatureModelView.Initialize();
        m_Interval = 60f;
        m_Coroutine = StartCoroutine(OnlineLoop());
    }

    public SettingModelView GetSettingModelView() => m_SettingModelView;
    public MasterModelView GetMasterModelView() => m_MasterModelView;
    public StageModelView GetStageModelView() => m_StageModelView;
    public FeatureModelView GetFeatureModelView() => m_FeatureModelView;
    public ShopModelView GetShopModelView() => m_ShopModelView;
    public AdsModelView GetAdsModelView() => m_AdsModelView;

    public SettingModel GetSettingData() => m_SettingModel;
    public MasterModel GetMasterData() => m_MasterModel;
    public FeatureModel GetFeatureData() => m_FeatureModel;
    public AdsModel GetAdsData() => m_AdsModel;
}
