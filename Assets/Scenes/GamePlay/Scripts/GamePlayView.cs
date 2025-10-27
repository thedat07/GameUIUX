
using UnityEngine;
using Creator;
using TMPro;
using UnityUtilities;
using Core.Events;
using UnityEngine.UI;
using Core;
using System.Collections;
using UnityEngine.Events;
using UniRx;

public partial class GamePlayController
{
    [Header("View")]

    [SerializeField] TextMeshProUGUI[] m_TxtLevel;
    [SerializeField] TextMeshProUGUI m_TxtTime;
    [SerializeField] Image m_Glow;
    [SerializeField] Image m_PanelTxtTime;
    [SerializeField] Sprite m_BlueTime;
    [SerializeField] Sprite m_RedTime;

    [SerializeField] Animation m_AnimationTxtTime;
    [SerializeField] Animation m_AnimationGlow;
    [SerializeField] SpineAnimUIUXController m_AnimTime;

    private bool m_ActiveTimeEffect = false;
    private bool m_ActiveTimeProgressEffect = false;
    private bool m_ActiveTimeGlowEffect = false;

    [Header("View Hard")]
    [SerializeField] GameObject m_HardView;
    [SerializeField] GameObject m_SuperHardView;

    [Header("View Freeze")]
    [SerializeField] GameObject m_FreezeView;
    [SerializeField] Slider m_ProgressSlider;
    [SerializeField] CanvasGroup m_CanvasGroupFreeze;

    private float m_CooldownTime = 10f;
    private float m_LastTriggerTime;
    private Coroutine m_CooldownCoroutine;

    private Image m_ProgressView;

    private Image GetProgressView()
    {
        if (!m_ProgressView)
        {
            m_ProgressView = m_PanelTxtTime.GetComponent<Image>();
        }
        return m_ProgressView;
    }

    void View()
    {
        m_CooldownTime = 10f;

        int level = GameManager.Instance.GetMasterData().dataStage.Get();

        foreach (var item in m_TxtLevel)
        {
            item.text = string.Format(string.Format("Level {0}", level));
        }

        GameEventSub.OnTimeInGameChange
            .Subscribe(tuple => ViewTime(tuple.a, tuple.b, tuple.c))
            .AddTo(this);

        GameEventSub.OnFreezeTimeChange
            .Subscribe(tuple => ViewFreeze(tuple.a, tuple.b, tuple.c))
            .AddTo(this);

        // GameEventSub.OnPeopleResolve
        //     .Subscribe(tuple => ViewArrow(tuple.people, tuple.hole, tuple.value))
        //     .AddTo(this);
    }

    void ViewType()
    {
        int type = GameManager.Instance.GetStageModelView().GetTypeLevel();
        ShowWarring(type);
    }

    public void ViewTime(float time, float toal, float deltaTime)
    {
        if (m_TxtTime)
        {
            m_TxtTime.text = DateTimeExtensions.ToTextTime(((int)time));
            CheckTime(time, 60, ActiveAnimTxtTime, DeactiveAnimTxtTime, ref m_ActiveTimeEffect);
            CheckTime(time, 30, ActiveAnimProgress, DeactiveAnimProgress, ref m_ActiveTimeProgressEffect);
            CheckTime(time, 20, ActiveAnimGlow, DeactiveAnimmGlow, ref m_ActiveTimeGlowEffect);

            foreach (var feature in updatables)
                feature.CustomUpdate(time);
        }

        void ActiveAnimTxtTime()
        {
            m_ActiveTimeEffect = true;
            m_AnimationTxtTime.Play("TimeGamePlay");
        }

        void DeactiveAnimTxtTime()
        {
            m_ActiveTimeEffect = false;
            m_AnimationTxtTime.Stop();
            m_TxtTime.color = Color.white;
            m_AnimationTxtTime.transform.localScale = Vector3.one;
        }

        void ActiveAnimProgress()
        {
            m_AnimTime.PlayIdle5ThenAction();
            m_ActiveTimeProgressEffect = true;
            m_PanelTxtTime.sprite = m_RedTime;
        }

        void DeactiveAnimProgress()
        {
            m_AnimTime.PlayActionOnly();
            m_ActiveTimeProgressEffect = false;
            m_PanelTxtTime.sprite = m_BlueTime;
            GetProgressView().color = Color.white;
        }

        void ActiveAnimGlow()
        {
            m_AnimationGlow.gameObject.SetActive(true);
            m_ActiveTimeGlowEffect = true;
            m_AnimationGlow.Play("TimeGamePlayGlow");
            m_AnimTime.PlayActionOnly();
        }

        void DeactiveAnimmGlow()
        {
            m_AnimationGlow.gameObject.SetActive(false);
            m_ActiveTimeGlowEffect = false;
            m_AnimationGlow.Stop();
            m_Glow.color = Color.white;
            m_AnimTime.PlayIdleOnly();
        }
    }

    void CheckTime(float time, float timeCheck, UnityAction active, UnityAction deactive, ref bool check)
    {
        if (time > timeCheck)
        {
            if (check)
                deactive?.Invoke();
        }
        else
        {
            if (!check)
                active?.Invoke();
        }
    }

    void ViewArrow()
    {
        StartCooldownArrow();
    }

    public void StartCooldownArrow()
    {
        if (m_CooldownCoroutine != null)
        {
            StopCoroutine(m_CooldownCoroutine);
        }
        m_CooldownCoroutine = StartCoroutine(CooldownRoutine());
    }

    private IEnumerator CooldownRoutine()
    {
        activeArrow.Value = false;
        m_LastTriggerTime = Time.time;
        while (Time.time < m_LastTriggerTime + m_CooldownTime)
        {
            float remaining = (m_LastTriggerTime + m_CooldownTime) - Time.time;
            yield return null;
        }
        activeArrow.Value = true;
        m_CooldownCoroutine = null;
    }

    void ShowWarring(int type)
    {
        foreach (var item in m_TxtLevel)
        {
            item.gameObject.SetActive(false);
        }
        m_TxtLevel[type].gameObject.SetActive(true);
        if (type == 1)
        {
            m_HardView.SetActive(true);
        }
        else if (type == 2)
        {
            m_SuperHardView.SetActive(true);
        }
    }

    void ViewFreeze(float time, float toal, float deltaTim)
    {
        m_FreezeView.SetActive(time > 0);
        if (time > 0)
        {
            float amout = time / toal * 1f;
            m_ProgressSlider.value = Mathf.Clamp(amout, 0, 1);
            if (time < 1)
            {
                m_CanvasGroupFreeze.alpha = Mathf.Clamp(time, 0, 1);
            }
        }
    }
}