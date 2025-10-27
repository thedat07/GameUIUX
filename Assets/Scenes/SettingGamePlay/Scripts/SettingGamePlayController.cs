using Creator;
using UnityEngine;
using TMPro;

public class SettingGamePlayController : Controller
{
    public const string SETTINGGAMEPLAY_SCENE_NAME = "SettingGamePlay";

    [SerializeField] TextMeshProUGUI m_TxtVer;

    [SerializeField] TextMeshProUGUI m_TxtId;

    public override string SceneName()
    {
        return SETTINGGAMEPLAY_SCENE_NAME;
    }

    void Start()
    {
        m_TxtVer.text = string.Format("Version: {0}", Application.version);
        m_TxtId.text = string.Format("ID: {0}", StaticData.GetPlayerIdSubstring());
    }

    public void OnSup()
    {
        Application.OpenURL(StaticData.WebSup);
    }

    public void OnRestore()
    {
        GameManager.Instance.GetShopModelView().OnRestore();
    }

    public void RedeemCode()
    {
        ManagerDirector.ReplaceScene(RedeemCodeController.REDEEMCODE_SCENE_NAME);
    }

    public void OnHome()
    {
        ManagerDirector.ReplaceScene(QuitGamePlayController.QUITGAMEPLAY_SCENE_NAME);
    }
}