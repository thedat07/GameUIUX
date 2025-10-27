using Creator;
using TMPro;
using UnityEngine.Events;


public enum TypeLose
{
    OutOfTime,
    NoSpace
}

public class LoseData
{
    public TypeLose typeLose;

    public UnityAction actionRevival;

    public LoseData(TypeLose typeLose, UnityAction actionRevival)
    {
        this.actionRevival = actionRevival;
        this.typeLose = typeLose;
    }
}

public class LoseController : Controller
{
    public const string LOSE_SCENE_NAME = "Lose";

    public override string SceneName()
    {
        return LOSE_SCENE_NAME;
    }

    LoseData m_Data;

    private bool m_IsRevival;

    public TextMeshProUGUI txtTime;

    public TextMeshProUGUI txtPrice;

    public SpineAnimUIUXController anim;

    void Awake()
    {
        Log();
    }

    void Log()
    {
        txtTime.text = string.Format("+ {0}s", StaticData.TimeRevive);
    }

    void Start()
    {
        anim.PlayIdle5ThenAction();
    }

    public override void OnActive(object data)
    {
        m_IsRevival = false;
        if (data != null)
        {
            m_Data = data as LoseData;
            string textTile = (m_Data.typeLose == TypeLose.OutOfTime ? "Out Of Time" : "No Space");
            //   txtTile.text = textTile;
            txtPrice.text = StaticData.CoinKeepPlaying.ToString();
        }
        GameManager.Instance.GetSettingModelView().PlaySound(TypeAudio.LevelFailed);
    }

    public void OnRevivalCoin()
    {
        GameManager.Instance.GetMasterModelView().PostMoney(StaticData.CoinKeepPlaying, StaticLogData.KeepPlaying, OnRevival, () =>
        {
            ManagerDirector.PushScene(ShopMiniController.SHOPMINI_SCENE_NAME);
        }, null);
    }

    public void OnRevival()
    {
        m_IsRevival = true;
        OnKeyBack();
    }

    public void OnClose()
    {
        if (!StaticDataFeature.ActiveEventTSC())
            ManagerDirector.ReplaceScene(RetryController.RETRY_SCENE_NAME, new RetryData("Failed!", "Try Again", 1, () => { ManagerDirector.RunScene(HomeController.HOME_SCENE_NAME); }, () => { }));
        else
            ManagerDirector.ReplaceScene(RetryController.RETRY_SCENE_NAME, new RetryData("You wil faill the Arcane Cube!", "Try Again", 2, () => { ManagerDirector.RunScene(HomeController.HOME_SCENE_NAME); }, () => { }));
    }

    public override void OnHidden()
    {
        if (m_IsRevival)
        {
            m_Data.actionRevival?.Invoke();
        }
    }
}