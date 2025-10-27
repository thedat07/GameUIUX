using System.Collections.Generic;
using UniRx;

public class FeatureModel
{
    public FeatureLukySpin featureLukySpin;
    public FeaturePiggyBank featurePiggyBank;
    public FeatureDailyRewradsAds featureDailyRewradsAds;
    public FeatureDailyRewradsFree featureDailyRewradsFree;
    public FeatureTheStickyCollector featureTSC;
    public FeatureStarterPack featureStarterPack;
    public FeaturePickMultiple featurePickMultiple;
    public FeatureRollingOffer featureRollingOffer;
    public FeatureSeasonPass featureSeasonPass;
    public FeatureWinStreak featureWin;

    private readonly List<IFeatureUpdatable> _updatables = new();

    public bool IsUnlock(int levelUnlock)
    {
        return GameManager.Instance.GetMasterData().dataStage.Get() >= levelUnlock;
    }

    public FeatureData GetData(TypeFeature type)
    {
        switch (type)
        {
            case TypeFeature.PiggyBank:
                return featurePiggyBank;

            case TypeFeature.LuckySpin:
                return featureLukySpin;

            case TypeFeature.DailyRewardsAds:
                return featureDailyRewradsAds;

            case TypeFeature.DailyRewardsFree:
                return featureDailyRewradsFree;

            case TypeFeature.TheStickyCollector:
                return featureTSC;

            case TypeFeature.PickMultiple:
                return featurePickMultiple;

            case TypeFeature.RollingOffer:
                return featureRollingOffer;

            case TypeFeature.SeasonPass:
                return featureSeasonPass;

            case TypeFeature.WinStreak:
                return featureWin;
        }

        return null;
    }

    void InitFeatureRemoteConfig()
    {
        foreach (var kvp in StaticDataFeature.FeatureUnlockLevels)
        {
            if (!IsUnlock(kvp.Value)) continue;
            if (!StaticDataFeature.FeatureActive.TryGetValue(kvp.Key, out bool isActive) || !isActive)
                continue;

            switch (kvp.Key)
            {
                case TypeFeature.PiggyBank:
                    if (featurePiggyBank == null)
                    {
                        featurePiggyBank = new FeaturePiggyBank(kvp.Key, kvp.Value);
                        _updatables.Add(featurePiggyBank);
                    }
                    break;

                case TypeFeature.LuckySpin:
                    if (featureLukySpin == null)
                    {
                        featureLukySpin = new FeatureLukySpin(kvp.Key, kvp.Value);
                        _updatables.Add(featureLukySpin);
                    }
                    break;

                case TypeFeature.DailyRewardsAds:
                    {
                        if (featureDailyRewradsAds == null)
                        {
                            featureDailyRewradsAds = new FeatureDailyRewradsAds(kvp.Key, kvp.Value);
                            _updatables.Add(featureDailyRewradsAds);
                        }

                        if (featureDailyRewradsFree == null)
                        {
                            featureDailyRewradsFree = new FeatureDailyRewradsFree(TypeFeature.DailyRewardsFree, kvp.Value);
                            _updatables.Add(featureDailyRewradsFree);
                        }

                    }
                    break;

                case TypeFeature.TheStickyCollector:
                    if (featureTSC == null)
                    {
                        featureTSC = new FeatureTheStickyCollector(kvp.Key, kvp.Value);
                        _updatables.Add(featureTSC);
                    }
                    break;

                case TypeFeature.PickMultiple:
                    if (featurePickMultiple == null)
                    {
                        featurePickMultiple = new FeaturePickMultiple(kvp.Key, kvp.Value);
                        _updatables.Add(featurePickMultiple);
                    }
                    break;

                case TypeFeature.RollingOffer:
                    if (featureRollingOffer == null)
                    {
                        featureRollingOffer = new FeatureRollingOffer(kvp.Key, kvp.Value);
                        _updatables.Add(featureRollingOffer);
                    }
                    break;

                case TypeFeature.SeasonPass:
                    if (featureSeasonPass == null)
                    {
                        featureSeasonPass = new FeatureSeasonPass(kvp.Key, kvp.Value);
                        _updatables.Add(featureSeasonPass);
                    }
                    break;

                case TypeFeature.WinStreak:
                    if (featureWin == null)
                    {
                        featureWin = new FeatureWinStreak(kvp.Key, kvp.Value);
                        _updatables.Add(featureWin);
                    }
                    break;
            }
        }
    }

    public void InitFeature()
    {
        featureStarterPack = new FeatureStarterPack(TypeFeature.StarterPack);
        _updatables.Add(featureStarterPack);
    }

    public FeatureModel()
    {
        InitFeature();
        InitFeatureRemoteConfig();
        GameManager.Instance.GetMasterData().dataStage.Value.Subscribe((x) => { InitFeatureRemoteConfig(); });
    }

    public void Refresh()
    {
        InitFeatureRemoteConfig();
    }

    public void CustomUpdate()
    {
        foreach (var feature in _updatables)
        {
            feature.CustomUpdate();
        }
    }
}