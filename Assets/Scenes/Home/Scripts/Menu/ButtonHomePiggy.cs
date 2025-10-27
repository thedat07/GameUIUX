using UnityEngine;
using UniRx;
using TMPro;
using UnityEngine.UI;
using UnityUtilities;
using UnityTimer;
using UnityUtils;

public class ButtonHomePiggy : ButtonHomeFeature
{
    protected override TypeFeature GetTypeFeature()
    {
        return TypeFeature.PiggyBank;
    }

    protected override FeatureData GetData()
    {
        return GameManager.Instance.GetFeatureData().featurePiggyBank;
    }

    [Header("View")]
    [SerializeField] TextMeshProUGUI txtTile;

    [SerializeField] GameObject glow;

    [SerializeField] Slider slider;

    [SerializeField] GameObject iconNotFull;

    [SerializeField] GameObject iconNoti;

    [SerializeField] SpineAnimUIUXController m_Anim;

    FeaturePiggyBank m_Data => GameManager.Instance.GetFeatureData().featurePiggyBank;

    private Timer timerUpdate;

    protected override void ViewLock()
    {
        glow.SetActive(false);
        txtTile.text = GetLevelLock();
        slider.FillAmount(0);
        iconNotFull.SetActive(true);
        m_Anim.gameObject.SetActive(false);
        iconNoti.SetActive(false);
    }

    protected override void ViewUnlockLock()
    {
        iconNotFull.SetActive(false);
        glow.SetActive(true);
        m_Data.GetData().exp.Subscribe(newExp => ViewExp(newExp)).AddTo(this);
        m_Data.GetData().isFull.Subscribe(isFull => ViewFull(isFull)).AddTo(this);
        ViewExp(m_Data.GetData().exp.Value);
        ViewFull(m_Data.GetData().isFull.Value);
        m_Anim.gameObject.SetActive(true);
    }

    protected override void Click()
    {
        m_Anim.PlayIdleThenActionClick(PlayAnim);
        HomeController.Instance.OnPiggy();
    }

    void ViewExp(int newExp)
    {
        int maxExp = m_Data.GetMaxExp();

        if (txtTile)
            txtTile.text = $"{newExp}/{maxExp}";

        if (slider)
            slider.FillAmount((float)newExp / maxExp);
    }

    void ViewFull(bool isFull)
    {
        iconNoti.SetActive(m_Data.IsNoti());

        PlayAnim();

        if (isFull)
        {
            if (slider) slider.value = 1f;

            if (timerUpdate != null) timerUpdate.Cancel();

            ViewTime();

            timerUpdate = Timer.Register(0.5f, ViewTime, isLooped: true, autoDestroyOwner: this);

            void ViewTime()
            {
                System.TimeSpan timerToShow = m_Data.GetRemainingTime();
                string formatD = "{0}d {1}h"; string formatH = "{0}h {1}m";
                string formatM = "{0}m {1}s"; string formatZero = "--";
                txtTile.text = CountdownTextUtilities.FormatCountdownLives(timerToShow, formatD, formatH, formatM, formatZero);
            }
        }
        else
        {
            if (timerUpdate != null) timerUpdate.Cancel();
        }
    }

    private bool m_Init;

    void PlayAnim()
    {
        if (GameManager.Instance.GetMasterModelView().IsPlay)
        {
            m_Anim.PlayActionOnly();
        }
        else
        {
            if (HomeController.Instance.IsWin() && !m_Init && m_Data.IsFull())
            {
                m_Anim.PlayActionOnly();
                m_Init = true;
            }
            else
            {
                AnimLoop();
            }
        }

        void AnimLoop()
        {
            if (m_Data.IsFull())
            {
                m_Anim.PlayIdleThenAction();
            }
            else
            {
                m_Anim.PlayIdle5ThenAction();
            }
        }
    }
}

