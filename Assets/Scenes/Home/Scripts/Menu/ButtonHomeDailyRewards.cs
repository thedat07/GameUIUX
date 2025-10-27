using UnityEngine;
using UniRx;
using TMPro;
using Coffee.UIEffects;

public class ButtonHomeDailyRewards : ButtonHomeFeature
{
    protected override TypeFeature GetTypeFeature()
    {
        return TypeFeature.DailyRewardsAds;
    }

    protected override FeatureData GetData()
    {
        return GameManager.Instance.GetFeatureData().featureDailyRewradsAds;
    }

    [Header("View")]
    [SerializeField] TextMeshProUGUI txtTile;
    [SerializeField] GameObject m_Noti;
    [SerializeField] TextMeshProUGUI m_txtNoti;
    [SerializeField] SpineAnimUIUXController m_anim;

    FeatureDailyRewradsFree m_DataFree => GameManager.Instance.GetFeatureData().featureDailyRewradsFree;
    FeatureDailyRewradsAds m_DataAds => GameManager.Instance.GetFeatureData().featureDailyRewradsAds;

    protected override void ViewLock()
    {
        txtTile.text = GetLevelLock();
        m_Noti.SetActive(false);
        m_anim.gameObject.SetActive(false);
    }

    protected override void ViewUnlockLock()
    {
        m_anim.gameObject.SetActive(true);
        txtTile.text = "Daily";
        m_DataFree.GetData().isClaim.Subscribe(x => { Noti(); }).AddTo(this);
        m_DataAds.GetData().claimIndex.Subscribe(x => { Noti(); }).AddTo(this);
        Noti();
    }

    void Noti()
    {
        int count = m_DataAds.CountNoti() + m_DataFree.CountNoti();
        m_txtNoti.text = count.ToString();
        m_Noti.SetActive(count > 0);
        if (count > 0)
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
        HomeController.Instance.OnDailyRewards();
    }
}
