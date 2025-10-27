using UnityEngine;
using UniRx;
using TMPro;

public class ButtonHomeSpin : ButtonHomeFeature
{
    protected override TypeFeature GetTypeFeature()
    {
        return TypeFeature.LuckySpin;
    }

    protected override FeatureData GetData()
    {
        return GameManager.Instance.GetFeatureData().featureLukySpin;
    }

    [Header("View")]
    [SerializeField] TextMeshProUGUI txtTile;

    [SerializeField] GameObject m_Noti;

    [SerializeField] TextMeshProUGUI m_txtNoti;

    [SerializeField] SpineAnimUIUXController m_anim;

    FeatureLukySpin m_Data => GetData() as FeatureLukySpin;

    protected override void ViewLock()
    {
        txtTile.text = GetLevelLock();
        m_Noti.SetActive(false);
        m_anim.gameObject.SetActive(false);
    }

    protected override void ViewUnlockLock()
    {
        m_anim.gameObject.SetActive(true);
        txtTile.text = "Lucky";
        m_Data.GetData().adsSpinsUsed.Subscribe(x => { Noti(); }).AddTo(this);
        m_Data.GetData().freeSpinUsed.Subscribe(x => { Noti(); }).AddTo(this);
        Noti();
    }

    void Noti()
    {
        var (freeLeft, adsLeft) = m_Data.GetRemainingSpins();
        int count = freeLeft + adsLeft;
        bool active = count > 0;
        m_Noti.SetActive(active);
        m_txtNoti.text = string.Format("{0}", count);

        if (active)
        {
            m_anim.PlayIdleThenAction();
        }
        else
        {
            m_anim.PlayIdle5ThenAction();
        }
    }

    protected override void Click()
    {
        m_anim.PlayIdleThenActionClick(Noti);
        HomeController.Instance.OnSpin();
    }
}
