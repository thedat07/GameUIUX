using Creator;
using TMPro;
using UnityEngine;

public class QuitGamePlayController : Controller
{
    public const string QUITGAMEPLAY_SCENE_NAME = "QuitGamePlay";

    public override string SceneName()
    {
        return QUITGAMEPLAY_SCENE_NAME;
    }

    public TextMeshProUGUI txtLevel;

    public GameObject[] type;

    void Start()
    {
        txtLevel.text = string.Format("Level {0}", GameManager.Instance.GetMasterData().dataStage.Get());

        if (!StaticDataFeature.ActiveEventTSC())
            type[0].SetActive(true);
        else
            type[1].SetActive(true);
    }

    public void OnQuit()
    {
        GameManager.Instance.GetAdsModelView().ShowInterstitial("home");
        GameManager.Instance.RunHome();
    }
}