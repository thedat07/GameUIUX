using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityUtilities;
using UniRx;
using Sirenix.OdinInspector;
using UnityEngine.Events;

public class CoinView : MonoBehaviour
{
    [SerializeField] bool m_ReplaceScene;

    [SerializeField] TextMeshProUGUI txtInfo;

    private int m_Cur;

    private Sequence m_Sequence;

    private Sequence m_SequencePuch;

    [Header("View")]
    public bool activeButton = true;

    [ShowIf("AlwaysShow")]
    public bool showPopupShop = true;

    public Transform icon;

    public bool AlwaysShow() { return activeButton; }

    public GameObject btnPlus;

    private ButtonGame m_Btn;


    public UnityEvent onShow;

    public UnityEvent onClose;

    public ButtonGame GetBtn()
    {
        if (m_Btn == null)
        {
            m_Btn = GetComponent<ButtonGame>();
        }
        return m_Btn;
    }

    void Start()
    {
        Init();

        GameManager.Instance.GetMasterData().dataMoney.Value.Subscribe(newVal => Effect(newVal)).AddTo(this);

        btnPlus.SetActive(activeButton);

        if (activeButton)
        {
            if (showPopupShop)
                GetBtn().OnClick.AddListener(OnClick);
        }
        else
        {
            GetBtn().interactable = false;
        }
    }

    void Init()
    {
        m_Cur = GameManager.Instance.GetMasterData().dataMoney.Get();
        View(m_Cur);
    }

    void Effect(int newValue)
    {
        int oldValue = m_Cur;
        m_Cur = newValue;

        if (m_Sequence != null && m_Sequence.IsActive())
        {
            m_Sequence.Kill();
        }

        m_Sequence = DOTween.Sequence();

        Tween tweenNumber = DOTween.To(
            () => oldValue,
            x =>
            {
                oldValue = x;
                View(oldValue);
            },
            newValue,
            0.25f // thời gian
        ).SetEase(Ease.Linear);

        // Add tween vào sequence
        m_Sequence.Append(tweenNumber);
    }

    void View(int number)
    {
        txtInfo.text = string.Format("{0}", number);
    }

    void OnClick()
    {
        onShow?.Invoke();

        if (m_ReplaceScene)
        {
            btnPlus.gameObject.SetActive(false);
            Creator.ManagerDirector.ReplaceScene(ShopMiniController.SHOPMINI_SCENE_NAME, null, null, () => { onClose?.Invoke(); btnPlus.gameObject.SetActive(true); });
        }
        else
        {
            Creator.ManagerDirector.PushScene(ShopMiniController.SHOPMINI_SCENE_NAME, null, null, () => { onClose?.Invoke(); });
        }
    }

    public void OnPunch()
    {
        if (m_SequencePuch != null && m_SequencePuch.IsActive())
        {
            m_SequencePuch.Kill();
        }

        m_SequencePuch = DOTween.Sequence();

        icon.transform.localScale = Vector3.one;

        m_SequencePuch.Append(icon.DOPunchScale(Vector3.one * 0.15f, 0.15f, 2, 0.5f));

        m_SequencePuch.SetLink(gameObject, LinkBehaviour.KillOnDestroy);

        GameManager.Instance.GetSettingModelView().PlaySound(TypeAudio.Coin);
    }
}
