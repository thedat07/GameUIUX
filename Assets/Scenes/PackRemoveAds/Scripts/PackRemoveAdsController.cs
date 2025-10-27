using Creator;

public class PackRemoveAdsController : Controller
{
    public const string PACKREMOVEADS_SCENE_NAME = "PackRemoveAds";

    public override string SceneName()
    {
        return PACKREMOVEADS_SCENE_NAME;
    }

    public SpineAnimUIUXController spineAnimUIUXController;

    void Start()
    {
        spineAnimUIUXController.PlayIdle5ThenAction();
    }
}