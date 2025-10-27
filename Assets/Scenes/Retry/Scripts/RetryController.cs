using Creator;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class RetryData
{
    public string txtInfo;

    public string txtBtn;

    public UnityAction onX;

    public UnityAction onClick;

    public int typeObject = 0;

    public RetryData(string txtInfo, string txtBtn, int typeObject, UnityAction onX, UnityAction onClick)
    {
        this.txtInfo = txtInfo;
        this.txtBtn = txtBtn;
        this.onX = onX;
        this.onClick = onClick;
        this.typeObject = typeObject;
    }
}

public class RetryController : Controller
{
    public const string RETRY_SCENE_NAME = "Retry";

    public override string SceneName()
    {
        return RETRY_SCENE_NAME;
    }

    public TextMeshProUGUI txtLevel;

    public TextMeshProUGUI txtInfo;

    public TextMeshProUGUI txtBtn;

    private RetryData m_Data;

    public GameObject heartView;

    public GameObject[] typeObject;

    public override void OnActive(object data)
    {
        if (data != null)
        {
            m_Data = data as RetryData;
            txtInfo.text = m_Data.txtInfo;
            txtBtn.text = m_Data.txtBtn;
            foreach (var item in typeObject)
            {
                item.SetActive(false);
            }
            typeObject[m_Data.typeObject].SetActive(true);
        }
    }

    void Start()
    {
        txtLevel.text = string.Format("Level {0}", GameManager.Instance.GetMasterData().dataStage.Get());
    }

    public void OnClose()
    {
        if (m_Data == null)
        {
            OnKeyBack();
        }
        else
        {
            m_Data?.onX?.Invoke();
        }
    }

    public void OnTryGame()
    {
        if (GameManager.Instance.GetMasterModelView().CanPlay())
        {
            GamePlayController.Instance.typeStatusResult = TypeStatusResult.Retry;
            m_Data?.onClick?.Invoke();
            GameManager.Instance.RunPlay(GamePlayController.Instance.info.UpdatRetry());
        }
        else
        {
            heartView.SetActive(false);
            ManagerDirector.ReplaceScene(MoreLivesController.MORELIVES_SCENE_NAME, null, null, () =>
            {
                heartView.SetActive(true);
            });
        }
    }
}