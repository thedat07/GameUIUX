using Creator;
using Gley.EasyIAP;
using Sirenix.OdinInspector;
using UnityEngine;

public partial class HomeController
{
    public Transform pointCoin;

    public Transform pointPlay;

    public Transform pointHeart;

    [Button]
    public void OnDailyRewards()
    {
        Creator.ManagerDirector.PushScene(DailyRewardsController.DAILYREWARDS_SCENE_NAME);
    }

    [Button]
    public void OnTSC()
    {
        Creator.ManagerDirector.PushScene(TheStickyCollectorController.THESTICKYCOLLECTOR_SCENE_NAME);
    }

    [Button]
    public void OnSetting()
    {
        Creator.ManagerDirector.PushScene(SettingHomeController.SETTINGHOME_SCENE_NAME);
    }

    [Button]
    public void OnSpin()
    {
        Creator.ManagerDirector.PushScene(LuckySpinController.LUCKYSPIN_SCENE_NAME);
    }

    [Button]
    public void OnPiggy()
    {
        Creator.ManagerDirector.PushScene(PiggyBankController.PIGGYBANK_SCENE_NAME);
    }

    [Button]
    public void OnStarterPack()
    {
        Creator.ManagerDirector.PushScene(PackStarterPackController.PACKSTARTERPACK_SCENE_NAME);
    }

    [Button]
    public void OnRemoveAds()
    {
        bool isActive = !GameManager.Instance.GetShopModelView().HasReceivedReward(Gley.EasyIAP.ShopProductNames.RemoveAdsBundle.ToString());
        if (isActive)
            Creator.ManagerDirector.PushScene(PackRemoveAdsController.PACKREMOVEADS_SCENE_NAME);
    }

    [Button]
    public void OnPlay()
    {
        if (GameManager.Instance.GetMasterModelView().CanPlay())
        {
            GameManager.Instance.RunPlay();
        }
        else
        {
            ManagerDirector.PushScene(MoreLivesController.MORELIVES_SCENE_NAME, new MoreLivesData(OnKeyBack, OnKeyBack, false));
        }
    }
}