using System.Collections.Generic;
using Core;
using Creator;

public class WinData
{
    public Dictionary<GameColor, int> dataGame;

    public WinData(Dictionary<GameColor, int> dataGame)
    {
        this.dataGame = dataGame;
    }
}

public partial class WinController
{
    public override void OnActive(object data)
    {
        if (data != null)
        {
            m_Data = data as WinData;
        }

        UpdateDataItem(m_Data.dataGame);

        InitData();
    }

    void InitData()
    {
        FirebaseUserProperties.SetMaxLevel(GameManager.Instance.GetMasterData().dataStage.Get());
        AdjustEventMap.LogCompleteLevel(GameManager.Instance.GetMasterData().dataStage.Get());
        GameManager.Instance.GetMasterModelView().Post(1, MasterDataType.Stage);
        GameManager.Instance.GetMasterModelView().Post(1, MasterDataType.Lives);
        GameManager.Instance.GetSettingModelView().PlaySound(TypeAudio.LevelCompleted);
    }

    public void UpdateDataItem(Dictionary<GameColor, int> dataGame)
    {
        GameManager.Instance.GetFeatureModelView().PostGameColor(TypeFeature.TheStickyCollector, dataGame);
        GameManager.Instance.GetFeatureModelView().PostWon(TypeFeature.PiggyBank);
    }
}