using Creator;
using UnityEngine;
using UnityTimer;

public class HomeData
{
    public bool win;

    public HomeData()
    {
        win = false;
    }

    public HomeData(bool win)
    {
        this.win = win;
    }
}

public partial class HomeController : SingletonController<HomeController>
{
    public const string HOME_SCENE_NAME = "HomeCore";

    public override string SceneName()
    {
        return HOME_SCENE_NAME;
    }

    private HomeData m_Data;

    public bool IsWin() => m_Data == null ? false : m_Data.win;

    public override void OnActive(object data)
    {
        if (data != null)
        {
            m_Data = data as HomeData;
        }
        else
        {
            m_Data = new HomeData();
        }
    }

    void Start()
    {
        GameManager.Instance.GetSettingModelView().PlayMusic();

        Timer.Register(0.25f, () =>
        {
            Creator.Director.LoadingAnimation(false);
            ShowPopupFirst();
        }, autoDestroyOwner: this);
    }
}