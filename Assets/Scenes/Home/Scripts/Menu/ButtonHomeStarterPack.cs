using UnityEngine;
using UnityTimer;
using TMPro;
using UnityUtilities;

public class ButtonHomeStarterPack : MonoBehaviour
{
    private ButtonGame m_ButtonGame;

    public TextMeshProUGUI txtTime;

    private Timer timerUpdate;

    FeatureStarterPack featureStarterPack => GameManager.Instance.GetFeatureData().featureStarterPack;

    [SerializeField] SpineAnimUIUXController m_anim;

    private ButtonGame GetButton()
    {
        if (m_ButtonGame == null)
        {
            m_ButtonGame = GetComponent<ButtonGame>();
        }
        return m_ButtonGame;
    }

    void Start()
    {
        TigerForge.EventManager.StartListening(ShopModelView.Key, UpdateView);
        UpdateView();
        GetButton().OnClick.AddListener(() =>
        {
            HomeController.Instance.OnStarterPack();
        });
        m_anim.PlayIdleThenAction();
    }

    void OnDestroy()
    {
        m_anim.PlayIdleThenActionClick(null);
        TigerForge.EventManager.StopListening(ShopModelView.Key, UpdateView);
    }

    public virtual void UpdateView()
    {
        bool isActive = !GameManager.Instance.GetShopModelView()
            .HasReceivedReward(Gley.EasyIAP.ShopProductNames.StarterPack.ToString()) && featureStarterPack.IsShowing();

        gameObject.SetActive(isActive);

        if (isActive && featureStarterPack.IsShowing())
        {
            if (timerUpdate != null) timerUpdate.Cancel();

            timerUpdate = Timer.Register(0.5f, () =>
            {
                UpdateTimeText();
            }, isLooped: true, autoDestroyOwner: this);
        }
        else
        {
            if (timerUpdate != null) timerUpdate.Cancel();
            txtTime.text = "";
        }
    }

    private void UpdateTimeText()
    {
        var remain = featureStarterPack.GetRemainingTime();

        if (remain <= System.TimeSpan.Zero)
        {
            gameObject.SetActive(false);
            timerUpdate?.Cancel();
        }
        else
        {
            System.TimeSpan timerToShow = remain;
            string formatD = "{0}d {1}h"; string formatH = "{0}h {1}m";
            string formatM = "{0}m {1}s"; string formatZero = "--";
            txtTime.text = CountdownTextUtilities.FormatCountdownLives(timerToShow, formatD, formatH, formatM, formatZero);
        }
    }
}
