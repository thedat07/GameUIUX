using UnityEngine;
using Creator;
using UnityEngine.UI;
using System.Linq;

public class PopupToolController : Controller
{
    public const string POPUPTOOL_SCENE_NAME = "PopupTool";

    public override string SceneName()
    {
        return POPUPTOOL_SCENE_NAME;
    }

    public InputField inputField;
    public InputField inputFieldPass;
    public GameObject pass;
    public GameObject content;
    public GameObject ads;

    [Header("Buttons")]
    public Button btnLog;
    public Button btnExit1;
    public Button btnExit2;
    public Button btnHideUI;
    public Button btnLevel;
    public Button btnAds;
    public Button btnRemove;
    public Button btnLive;
    public Button btnMoney;
    public Button btnNextLevel;
    public Button btnWin;
    public Button btnLose;

    void Start()
    {
#if UNITY_EDITOR
        GameManager.Instance.GetMasterModelView().ActiveTest();
#endif

        if (GameManager.Instance.GetMasterModelView().IsTest)
        {
            pass.SetActive(false);
            content.SetActive(true);
        }
        else
        {
            pass.SetActive(true);
            content.SetActive(false);
        }
        SettingButton();
    }

    void SettingButton()
    {
        btnLog.onClick.AddListener(() => { Log(); });
        btnExit1.onClick.AddListener(() => { OnKeyBack(); });
        btnExit2.onClick.AddListener(() => { OnKeyBack(); });
        btnHideUI.onClick.AddListener(() => { BtnHideUI(); });
        btnLevel.onClick.AddListener(() => { SetLevel(); });
        btnAds.onClick.AddListener(() => { ShowAds(); });
        btnRemove.onClick.AddListener(() => { RemoveAds(); });
        btnMoney.onClick.AddListener(() => { AddMoney(); });
        btnLive.onClick.AddListener(() => { AddTimeInfinity(); });


        btnWin.gameObject.SetActive(GameManager.Instance.GetMasterModelView().IsPlay);
        btnLose.gameObject.SetActive(GameManager.Instance.GetMasterModelView().IsPlay);
        btnNextLevel.gameObject.SetActive(GameManager.Instance.GetMasterModelView().IsPlay);

        btnWin.onClick.AddListener(() => { OnWin(); });
        btnLose.onClick.AddListener(() => { OnLose(); });
        btnNextLevel.onClick.AddListener(() => { NextLevel(); });
    }

    public void Log()
    {
        if (inputFieldPass.text == "Sb_Game")
        {
            pass.SetActive(false);
            content.SetActive(true);
            GameManager.Instance.GetMasterModelView().ActiveTest();
        }
    }

    public void AddMoney()
    {
        GameManager.Instance.GetMasterModelView().Post(999999, MasterDataType.Money);
        GameManager.Instance.GetMasterModelView().Post(999, MasterDataType.Booster1);
        GameManager.Instance.GetMasterModelView().Post(999, MasterDataType.Booster2);
        GameManager.Instance.GetMasterModelView().Post(999, MasterDataType.Booster3);
    }

    public void NextLevel()
    {
        if (!GameManager.Instance.GetMasterModelView().IsPlay) return;
        GameManager.Instance.GetMasterModelView().Post(1, MasterDataType.Stage);
        bool isRandomColor = StaticData.IsRandomColor;
        int level = GameManager.Instance.GetStageModelView().GetLoopLevel();
        Creator.ManagerDirector.RunScene(GamePlayController.GAMEPLAY_SCENE_NAME, new GamePlaySceneData(level, isRandomColor, 0));
    }

    public void SetLevel()
    {
        if (inputField.text != "")
        {
            int number;
            if (int.TryParse(inputField.text, out number))
            {
                GameManager.Instance.GetMasterModelView().Put(int.Parse(inputField.text), MasterDataType.Stage);

                bool isRandomColor = StaticData.IsRandomColor;

                int level = GameManager.Instance.GetStageModelView().GetLoopLevel();

                Creator.ManagerDirector.RunScene(GamePlayController.GAMEPLAY_SCENE_NAME, new GamePlaySceneData(level, isRandomColor, 0));
            }
        }
    }

    public void ShowAds()
    {
        ads.gameObject.SetActive(true);
    }

    public void RemoveAds()
    {
        GameManager.Instance.GetAdsModelView().OnRemoveAds();
    }

    public void OnWin()
    {
        if (!GameManager.Instance.GetMasterModelView().IsPlay) return;
        GamePlayController.Instance.PopupWinTimeZero();
    }

    public void OnLose()
    {
        if (!GameManager.Instance.GetMasterModelView().IsPlay) return;
        GamePlayController.Instance.PopupLoseOutOfTime();
    }

    public void AddTimeInfinity()
    {
        GameManager.Instance.GetMasterModelView().Post(180, MasterDataType.LivesInfinity);
    }

    public void BtnHideUI()
    {
        string[] scenesName = new string[] { };
        GameManager.Instance.hideUI = !GameManager.Instance.hideUI;
        var lst = FindObjectsOfType<Controller>();
        foreach (var item in lst)
        {
            if (item != this || scenesName.Any(x => x == item.SceneName()))
            {
                item.HideUI();
            }
        }
    }
}