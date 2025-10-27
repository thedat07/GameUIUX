using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using R3;

public partial class ButtonBoosterManager : MonoBehaviour
{
    [ValueDropdown("@HelperCreator.BoosterTypes")]
    public MasterDataType type;

    public Image iconBG;

    [Header("View")]
    public GameObject viewObject;
    public TextMeshProUGUI txtInfo;
    public GameObject noti;
    public GameObject plus;

    [SerializeField] protected GameObject m_Arrow;
    Animation m_ArrowAnim;

    [Header("Lock")]
    public GameObject lockObject;
    public TextMeshProUGUI txtLevelLock;

    ButtonGame m_ButtonGame;

    CanvasGroup m_CanvasGroup;

    protected BoosterData GetBoosterData() => GameManager.Instance.GetMasterData().GetBoosterData(type);

    string GetUseLog() => string.Format("use_booster_{0}", type);

    string GetCloseLog() => string.Format("close_booster_{0}", type);

    protected ButtonGame GetButtonGame()
    {
        if (m_ButtonGame == null)
        {
            m_ButtonGame = GetComponent<ButtonGame>();
        }
        return m_ButtonGame;
    }

    protected CanvasGroup GetCanvasGroup()
    {
        if (m_CanvasGroup == null)
        {
            m_CanvasGroup = GetComponent<CanvasGroup>();
        }
        return m_CanvasGroup;
    }

    void Start()
    {
        GamePlayController.Instance.OnInit.AddListener(Init);
    }

    void IsPauseGame(bool click)
    {
        GetButtonGame().interactable = !click;
    }

    protected virtual void Init()
    {
        ViewInit();
        HandleInputInit();
        //     Manager.GameManager.Instance.IsPauseGame.Subscribe(IsPauseGame).AddTo(this);
        // IsPauseGame(Manager.GameManager.Instance.IsPauseGame.Value);
    }

    public virtual void OnClose()
    {

    }

    protected virtual void OnDestroy()
    {

    }
}
