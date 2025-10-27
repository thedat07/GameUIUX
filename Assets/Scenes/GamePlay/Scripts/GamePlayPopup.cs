using Creator;
using Sirenix.OdinInspector;
using Core.Events;
using UnityTimer;
using System.Linq;
using UniRx;

public partial class GamePlayController
{
    private bool m_ActivePopupLose = false;

    private bool m_ActivePopupWin = false;

    public SoNewFeature soNewFeature;

    void InitPopup()
    {
        m_ActivePopupLose = false;
        m_ActivePopupWin = false;

        GameEventSub.OnLevelWin.Subscribe(tuple => { PopupWin(); }).AddTo(this);

        GameEventSub.OnLevelLose.Subscribe(tuple => { PopupLoseOutOfTime(); }).AddTo(this);

        PopupNewFeature();
    }

    void PopupNewFeature()
    {
        if (StaticData.IsUnLocNewFeature)
        {
            int level = GameManager.Instance.GetMasterData().dataStage.Get();
            SoNewFeature.Data data = soNewFeature.datas.FirstOrDefault(x => x.requireLevel == level);
            if (data != null)
            {
                Creator.ManagerDirector.PushScene(UnLockNewFeatureController.UNLOCKNEWFEATURE_SCENE_NAME, new UnLockNewFeatureData(data));
            }
        }
        StaticData.IsUnLocNewFeature = false;
    }

    public void PopupWin(WinData data, float time = 1.5f)
    {
        if (!m_ActivePopupWin)
        {
            typeStatusResult = TypeStatusResult.Won;
            m_ActivePopupWin = true;
            Animation.GetCanvasGroup().blocksRaycasts = false;
            Timer.Register(time, () => { ManagerDirector.PushScene(WinController.WIN_SCENE_NAME, data); }, autoDestroyOwner: this);
        }
    }

    public void PopupLose(LoseData data)
    {
        if (!m_ActivePopupLose)
        {
            m_ActivePopupLose = true;
            typeStatusResult = TypeStatusResult.Lose;
            ManagerDirector.PushScene(LoseController.LOSE_SCENE_NAME, data);
        }
    }

    public void PopupSetting()
    {
        LevelManager.Instance.SetPause(true);
        ManagerDirector.PushScene(SettingGamePlayController.SETTINGGAMEPLAY_SCENE_NAME, null, null, () =>
        {
            LevelManager.Instance.SetPause(false);
        });
    }

    public void PopupRetry()
    {
        LevelManager.Instance.SetPause(true);

        RetryData data;

        if (!StaticDataFeature.ActiveEventTSC())
            data = new RetryData("You will lose 1 life!", "Retry", 0, OnKeyBack, () => { });
        else
            data = new RetryData("You wil faill the Arcane Cube!", "Retry", 3, OnKeyBack, () => { });

        ManagerDirector.PushScene(RetryController.RETRY_SCENE_NAME, data, null, () =>
        {
            LevelManager.Instance.SetPause(false);
        });
    }

    [Button]
    public void PopupLoseOutOfTime()
    {
        PopupLose(new LoseData(TypeLose.OutOfTime, () =>
        {
            LevelManager.Instance.TryAddTime(20f);
            LevelManager.Instance.SetPause(false);
            m_ActivePopupLose = false;
        }));
    }

    [Button]
    public void PopupLoseNoSpace()
    {
        PopupLose(new LoseData(TypeLose.NoSpace, () =>
        {
            LevelManager.Instance.TryAddTime(20f);
            LevelManager.Instance.SetPause(false);
            m_ActivePopupLose = false;
        }));
    }

    [Button]
    public void PopupWin()
    {
        GameManager.Instance.GetMasterModelView().infoGame.Win();
        PopupWin(new WinData(new System.Collections.Generic.Dictionary<GameColor, int>()));
    }

    public void PopupWinTimeZero()
    {
        PopupWin(new WinData(new System.Collections.Generic.Dictionary<GameColor, int>()), 0);
    }
}