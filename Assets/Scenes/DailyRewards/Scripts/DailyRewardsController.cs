using Creator;
using UniRx;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DailyRewardsController : Controller
{
    public const string DAILYREWARDS_SCENE_NAME = "DailyRewards";

    public override string SceneName()
    {
        return DAILYREWARDS_SCENE_NAME;
    }

    FeatureDailyRewradsFree GetDataFree() => GameManager.Instance.GetFeatureData().featureDailyRewradsFree;

    FeatureDailyRewradsAds GetDataAds() => GameManager.Instance.GetFeatureData().featureDailyRewradsAds;

    [Header("Info")]
    public Slider progress;
    public float[] vauleProgres;

    public RewardsItemView rewardsItemViewFree;
    public SoStoreRewards soDailyFree;

    public RewardsItemView[] rewardsItemViewAds;
    public SoStoreRewards soDailyAds;

    void Start()
    {
        View(GetDataAds().GetData().claimIndex.Value);
        GetDataAds().GetData().claimIndex.Subscribe(x => { View(x); }).AddTo(this);

        rewardsItemViewFree.Init(soDailyFree.datas[0].rewards[0], soDailyFree.datas[0].rewards[0].GetIcon());

        for (int i = 0; i < soDailyAds.datas.Length; i++)
        {
            rewardsItemViewAds[i].Init(soDailyAds.datas[i].rewards[0], soDailyAds.datas[i].rewards[0].GetIcon());
        }
    }

    public void View(int index)
    {
        progress.DOKill();
        progress.DOValue(vauleProgres[index], 0.25f).SetEase(Ease.Linear);
    }

    public void OnClaimFree()
    {
        OnRewards(soDailyFree.datas[0].rewards.ToList(), "free");
        GetDataFree().Claim();
    }

    public void OnClaimAds()
    {
        GameManager.Instance.GetAdsModelView().ShowRewardedVideo("DailyRewards", () =>
        {
            OnRewards(soDailyAds.datas[GetDataAds().GetClaimIndex()].rewards.ToList(), GetDataAds().GetClaimIndex().ToString(), MasterModelView.TypeSource.Ads);
            GetDataAds().Claim();
        });
    }

    private void OnRewards(List<InventoryItem> data, string log, MasterModelView.TypeSource type = MasterModelView.TypeSource.Free)
    {
        DataMethod r = new DataMethod(HelperCreator.Convert(data), "Daily rewards", StaticLogData.LogDailyRewards(log));
        r.Apply(typeSource: type);
        Creator.ManagerDirector.PushScene(RewardsController.REWARDS_SCENE_NAME, r);
    }
}