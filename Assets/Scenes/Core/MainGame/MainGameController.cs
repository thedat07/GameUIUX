using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityTimer;

public class MainGameController : MonoBehaviour
{
    public const string MAIN_GAME = "MainGameCore";

    void Awake()
    {
        Creator.Director director = new Creator.Director();

#if DEBUG
        Debug.unityLogger.logEnabled = true;
#else
        Debug.unityLogger.logEnabled = false;
#endif


       Debug.unityLogger.logEnabled = true;
    }

    void Start()
    {
        Creator.Director.SceneAnimationDuration = 0.2f;

        Creator.Director.LoadingSceneName = PopupLoadingController.SCENE_NAME;

        Timer.Register(Creator.Director.SceneAnimationDuration, () =>
        {
            transform.GetChild(0).gameObject.SetActive(false);
            if (GameManager.Instance.GetMasterModelView().CanPlay())
            {
                GameManager.Instance.PlayNext(false);
            }
            else
            {
                Creator.Director.RunScene(HomeController.HOME_SCENE_NAME);
            }
        }, autoDestroyOwner: this);
    }
}
