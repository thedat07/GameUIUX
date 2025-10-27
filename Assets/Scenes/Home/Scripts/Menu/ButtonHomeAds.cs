using UnityEngine;
using UniRx;
using Gley.EasyIAP.Internal;

public class ButtonHomeAds : MonoBehaviour
{
    private ButtonGame m_ButtonGame;

    private ButtonGame GetButton()
    {
        if (m_ButtonGame == null)
        {
            m_ButtonGame = GetComponent<ButtonGame>();
        }
        return m_ButtonGame;
    }

    [SerializeField] SpineAnimUIUXController m_anim;

    void Start()
    {
        TigerForge.EventManager.StartListening(ShopModelView.Key, UpdateView);
        UpdateView();
        GetButton().OnClick.AddListener(OnClick);
        m_anim.PlayIdleThenAction();
    }

    void OnDestroy()
    {
        TigerForge.EventManager.StopListening(ShopModelView.Key, UpdateView);
    }

    void OnClick()
    {
        m_anim.PlayIdleThenActionClick(m_anim.PlayIdleThenAction);
        HomeController.Instance.OnRemoveAds();
    }

    public virtual void UpdateView()
    {
        bool isActive = !GameManager.Instance.GetShopModelView().HasReceivedReward(Gley.EasyIAP.ShopProductNames.RemoveAdsBundle.ToString());
        gameObject.SetActive(isActive);
    }
}
