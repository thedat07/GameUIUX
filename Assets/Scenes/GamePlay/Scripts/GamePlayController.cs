using System.Collections.Generic;
using Creator;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;
using UnityTimer;

public class GamePlaySceneData
{
    public int level;
    public bool randomColor;
    public int reTry;

    public GamePlaySceneData(int level, bool randomColor, int reTry)
    {
        this.level = level;
        this.randomColor = randomColor;
        this.reTry = reTry;
    }
}

[System.Serializable]
public class GamePlayInfo
{
    public class DataInfoBooster
    {
        public int useBooster1;
        public int useBooster2;
        public int useBooster3;

        public DataInfoBooster()
        {
            useBooster1 = 0;
            useBooster2 = 0;
            useBooster3 = 0;
        }
    }

    public int levelId;

    public int retry;

    public float startTime;

    public DataInfoBooster dataInfo;

    public void Init(int retry = 0)
    {
        levelId = GameManager.Instance.GetMasterData().dataStage.Get();
        startTime = Time.time;
        dataInfo = new DataInfoBooster();
        this.retry = retry;
    }

    public int GetDuration() => (int)(Time.time - startTime);

    public string GetInfo() => JsonUtility.ToJson(dataInfo);

    public int UpdatRetry() => retry + 1;

    public int GetRetry(bool isRetry) => isRetry ? (retry + 1) : retry;
}

public enum TypeStatusResult
{
    Won = 1,
    Lose = 2,
    Quit = 3,
    Retry = 4,
    OutTime = 5,
}

public interface IPeopleUpdatable
{
    void CustomUpdate(float time);
}

public partial class GamePlayController : SingletonController<GamePlayController>
{
    public const string GAMEPLAY_SCENE_NAME = "GamePlayCore";

    public override string SceneName()
    {
        return GAMEPLAY_SCENE_NAME;
    }

    GamePlaySceneData m_Data;

    public GamePlayInfo info;

    public int GetLevel() => m_Data.level;

    public bool IsRandomColor() => m_Data.randomColor;

    public UnityEvent OnInit;

    public BoolReactiveProperty activeArrow;

    public List<IPeopleUpdatable> updatables = new();

    public TypeStatusResult typeStatusResult;

    public override void OnActive(object data)
    {
        if (data != null)
        {
            m_Data = data as GamePlaySceneData;
        }
        activeArrow.Value = false;
        info = new GamePlayInfo();

        if (data != null)
            info.Init(m_Data.reTry);
        else
            info.Init();
    }

    void Start()
    {
        typeStatusResult = TypeStatusResult.Quit;

        GameManager.Instance.GetMasterModelView().IsPlay = true;

        GameManager.Instance.GetMasterModelView().infoGame.countPlay += 1;

        UpdateData();

        GameManager.Instance.GetSettingModelView().PlayMusic(1);

        InitPopup();

        View();

        FirebaseEventLogger.LogLevelStart(info);

        this.OnDestroyAsObservable()
        .Subscribe(_ => { QuitGame(); })
        .AddTo(GameManager.Instance);


        Creator.Director.LoadingAnimation(false);
    }

    void UpdateData()
    {
        GameManager.Instance.GetMasterModelView().ConsumeLife();
        GameManager.Instance.GetFeatureModelView().ClearDataWin();
    }

    public void PlayGame()
    {
        Timer.Register(0.2f, () =>
        {
            ViewTime(0, 0, 0);
            Creator.Director.LoadingAnimation(false);
            ViewType();
            OnInit?.Invoke();
        }, autoDestroyOwner: this);
    }

    void QuitGame()
    {
        GameManager.Instance.GetMasterModelView().IsPlay = false;

        if (!m_ActivePopupWin)
        {
            GameManager.Instance.GetMasterModelView().infoGame.Lose();
        }
        else
        {
            GameManager.Instance.GetAdsModelView().ShowInterstitial("won");
        }

        EventResult();

        Console.LogWarning("Quit Game Play");
    }

    public void EventResult()
    {
        FirebaseEventLogger.LogLevelEnd((int)typeStatusResult, info);
    }
}