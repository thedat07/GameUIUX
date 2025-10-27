using Creator;
using TMPro;
using UnityEngine;

public class SettingHomeController : Controller
{
    public const string SETTINGHOME_SCENE_NAME = "SettingHome";

    public override string SceneName()
    {
        return SETTINGHOME_SCENE_NAME;
    }

    [SerializeField] TextMeshProUGUI m_TxtVer;

    [SerializeField] TextMeshProUGUI m_TxtId;


    // public ButtonGame btnFb;

    void Start()
    {
        m_TxtVer.text = string.Format("Version: {0}", Application.version);
        //   ViewBtnFB();
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

    public void OnJoinFacebook()
    {
        if (!StaticData.JoinFacebook)
        {
         //   Application.OpenURL(facebookUrl);
            GameManager.Instance.GetMasterModelView().Post(100, MasterDataType.Money, StaticLogData.JoinFacebook);
            StaticData.JoinFacebook = true;
            //  ViewBtnFB();
        }
    }

    public void RedeemCode()
    {
        ManagerDirector.ReplaceScene(RedeemCodeController.REDEEMCODE_SCENE_NAME);
    }
}