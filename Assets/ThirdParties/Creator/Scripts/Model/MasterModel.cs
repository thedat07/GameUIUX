using System.Collections.Generic;
using UnityEngine.Events;
using UnityUtilities;

public enum MasterDataType
{
    Stage = 0,
    Money = 1,

    Booster1 = 2,
    Booster2 = 3,
    Booster3 = 4,
    X2Item = 5,
    Profile = 6,

    NoAds = 12,
    None = 13,
    LivesInfinity = 14,
    Lives = 15,

}

public class MasterModel
{
    public IntMasterData dataStage;
    public IntMasterData dataMoney;

    public BoosterData dataBooster1;
    public BoosterData dataBooster2;
    public BoosterData dataBooster3;

    public ProfileData dataProfileData;

    public MasterModel()
    {
        dataStage = new IntMasterData(1, MasterDataType.Stage);
        dataMoney = new IntMasterData(0, MasterDataType.Money);

        dataProfileData = new ProfileData(new List<int>() { 0, 1, 2 }, MasterDataType.Profile);

        dataBooster1 = new BoosterData(0, MasterDataType.Booster1, 6);
        dataBooster2 = new BoosterData(0, MasterDataType.Booster2, 10);
        dataBooster3 = new BoosterData(0, MasterDataType.Booster3, 12);
    }

    public BoosterData GetBoosterData(MasterDataType type)
    {
        BoosterData result = type switch
        {
            MasterDataType.Booster1 => dataBooster1,
            MasterDataType.Booster2 => dataBooster2,
            MasterDataType.Booster3 => dataBooster3,
            _ => null
        };
        return result;
    }

    public int GetData(MasterDataType type)
    {
        int result = type switch
        {
            MasterDataType.Stage => dataStage.Get(),
            MasterDataType.Money => dataMoney.Get(),


            MasterDataType.Booster1 => dataBooster1.Get(),
            MasterDataType.Booster2 => dataBooster2.Get(),
            MasterDataType.Booster3 => dataBooster3.Get(),
            _ => 0
        };
        return result;
    }

    public bool UnLock(MasterDataType type)
    {
        BoosterData boosterData = GetBoosterData(type);

        if (dataStage.Get() == boosterData.GetLevelUnlock() && boosterData.IsLock())
        {
            GameManager.Instance.GetMasterModelView().Post(2, type, StaticLogData.LogTutorial(type.ToString()), MasterModelView.TypeSource.Free);
            boosterData.UnLock();
            return true;
        }
        else if (dataStage.Get() > boosterData.GetLevelUnlock() && boosterData.IsLock())
        {
            boosterData.UnLock();
        }
        return false;
    }

    public void SaveRaw(string json)
    {
        ES3.SaveRaw(json, SaveExtensions.keyFileMasterData);
        dataStage = new IntMasterData(1, MasterDataType.Stage);
        dataMoney = new IntMasterData(0, MasterDataType.Money);
        dataProfileData = new ProfileData(new List<int>() { 0, 1, 2 }, MasterDataType.Profile);
        dataBooster1 = new BoosterData(0, MasterDataType.Booster1, 6);
        dataBooster2 = new BoosterData(0, MasterDataType.Booster2, 10);
        dataBooster3 = new BoosterData(0, MasterDataType.Booster3, 12);
        Creator.Director.Object.PlayEffect();
    }
}