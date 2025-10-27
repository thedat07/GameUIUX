using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ExaGames.Common.TimeBasedLifeSystem;
using Creator;
using UniRx;

public class HeartView : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_TxtInfo;
    [SerializeField] TextMeshProUGUI m_TxtLive;

    [SerializeField] GameObject m_LiveObjets;
    [SerializeField] GameObject m_LiveInfinityObjets;

    [Header("View")]
    public bool activeButton = true;

    public GameObject btnPlus;

    private ButtonGame m_Btn;

    public bool CanClick() => !GetLives().HasInfiniteLives && GetLives().Lives < GetLives().MaxLives && activeButton && !m_ShowPopup;

    public LivesManager GetLives() => GameManager.Instance.GetMasterModelView().livesManager;

    private bool m_ShowPopup;

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
        View();

        if (activeButton)
        {
            GetBtn().OnClick.AddListener(OnClick);
        }
        else
        {
            GetBtn().interactable = false;
        }
        UnityTimer.Timer.Register(0.5f, View, isLooped: true, autoDestroyOwner: this);
    }

    void View()
    {
        m_TxtInfo.text = string.Format("{0}", GetLives().RemainingTimeString);
        m_TxtLive.text = string.Format("{0}", GetLives().Lives);
        m_LiveObjets.SetActive(!GetLives().HasInfiniteLives);
        m_LiveInfinityObjets.SetActive(GetLives().HasInfiniteLives);
        btnPlus.SetActive(CanClick());
    }

    void OnClick()
    {
        if (CanClick())
        {
            m_ShowPopup = true;
            Creator.ManagerDirector.PushScene(MoreLivesController.MORELIVES_SCENE_NAME,
            new MoreLivesData(Creator.ManagerDirector.PopScene, Creator.ManagerDirector.PopScene, false),
            null, () =>
            {
                m_ShowPopup = false;
            });
        }
    }
}
