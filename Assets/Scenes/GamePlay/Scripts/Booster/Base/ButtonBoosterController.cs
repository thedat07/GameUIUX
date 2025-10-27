public partial class ButtonBoosterManager
{
    bool CanUseBooster()
    {
        switch (type)
        {
            case MasterDataType.Booster1:
                return CanUseBooster1();
            case MasterDataType.Booster2:
                return CanUseBooster2();
            case MasterDataType.Booster3:
                return CanUseBooster3();
        }

        return false;
    }

    protected bool CanUseBooster1()
    {
        return BoosterManager.Instance.CanUseBoosterFreezeClock();
    }

    protected bool CanUseBooster2()
    {
        return BoosterManager.Instance.CanUseBoosterPropeller();
    }

    protected bool CanUseBooster3()
    {
        return BoosterManager.Instance.CanUseBoosterMagnet();
    }

    void UseBooster()
    {
        GamePlayController.Instance.StartCooldownArrow();
        switch (type)
        {
            case MasterDataType.Booster1:
                {
                    IsUse();
                    UseBooster1();
                }
                break;
            case MasterDataType.Booster2:
                UseBooster2();
                break;
            case MasterDataType.Booster3:
                UseBooster3();
                break;
        }
    }

    public virtual void IsUse()
    {
        switch (type)
        {
            case MasterDataType.Booster1:
                GamePlayController.Instance.info.dataInfo.useBooster1 += 1;
                break;
            case MasterDataType.Booster2:
                GamePlayController.Instance.info.dataInfo.useBooster2 += 1;
                break;
            case MasterDataType.Booster3:
                GamePlayController.Instance.info.dataInfo.useBooster3 += 1;
                break;
        }

        GamePlayController.Instance.activeArrow.Value = false;

        GameManager.Instance.GetMasterModelView().Post(-1, type, GetUseLog());
    }

    protected virtual void UseBooster1()
    {
        BoosterManager.Instance.UseBooster(BoosterManager.BoosterType.Booster1);
    }

    protected virtual void UseBooster2()
    {
        BoosterManager.Instance.UseBooster(BoosterManager.BoosterType.Booster2);
    }

    protected virtual void UseBooster3()
    {
        BoosterManager.Instance.UseBooster(BoosterManager.BoosterType.Booster3);
    }
}