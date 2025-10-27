

using DesignPatterns;
using Sirenix.OdinInspector;
using UnityEngine;
using Core.Events;
using UniRx;

public class BoosterManager : Singleton<BoosterManager>
{
    public enum BoosterType
    {
        None,
        Booster1,
        Booster2,
        Booster3,
    }

    [field: SerializeField] public bool IsUsingBooster { get; set; }

    [field: SerializeField] public BoosterType CurrentBoosterType { get; set; }

    [Button]
    public void UseBooster(BoosterType boosterType)
    {
        CurrentBoosterType = boosterType;
        switch (boosterType)
        {
            case BoosterType.None:
                break;
            case BoosterType.Booster1:
                UseBoosterFreezeClock();
                break;
            case BoosterType.Booster2:
                UseBoosterPropeller();
                break;
            case BoosterType.Booster3:
                UseBoosterMagnet();
                break;
            default:
                break;
        }
        IsUsingBooster = true;
    }

    private void UseBoosterFreezeClock()
    {
        LevelManager.Instance.AddFreezeDuration(StaticData.TimeFrezze);
    }

    private void UseBoosterPropeller()
    {
        GameEventSub.OnUsePropeller.OnNext(Unit.Default);
    }
    private void UseBoosterMagnet()
    {
        GameEventSub.OnUseMagnet.OnNext(Unit.Default);
    }

    public bool CanUseBoosterFreezeClock()
    {
        return LevelManager.Instance.CanUseBoosterFreeze();
    }

    public bool CanUseBoosterPropeller()
    {
        return true;
    }

    public bool CanUseBoosterMagnet()
    {
        return true;
    }

    [Button]
    public void CancelBooster(BoosterType boosterType)
    {
        if (boosterType == BoosterType.Booster1) return;
        if (CurrentBoosterType != boosterType) return;
        switch (CurrentBoosterType)
        {
            case BoosterType.Booster2:
                GameEventSub.OnCancelBoosterMagnet.OnNext(Unit.Default);
                break;
            case BoosterType.Booster3:
                GameEventSub.OnCancelBoosterPropeller.OnNext(Unit.Default);
                break;
            default:
                break;
        }
        IsUsingBooster = false;
        CurrentBoosterType = BoosterType.None;
    }
}