using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemTSCView : MonoBehaviour
{
    private int m_Index;

    [Header("Info")]

    public TextMeshProUGUI txtInfo;

    public GameObject objectDone;

    public GameObject objectNotDone;

    public GameObject objectProgress;


    [Header("View")]
    public GameObject objectTick;

    public GameObject objectGlow;

    private SoStoreRewards.RewardData m_Data;

    public RewardsItemRView viewR;

    public RewardsItemView view;

    private SoGoal.ProgressData m_ProgressData;

    public void Init(int index, SoGoal soGoal, SoGoal.ProgressData progress, TheStickyCollectorController tsc)
    {
        m_Index = index;
        m_Data = soGoal.GetReward(m_Index);
        m_ProgressData = progress;

        if (viewR)
            viewR.Init(m_Data,tsc);
        else
            view.Init(m_Data.rewards[0]);

        ViewInit();
    }

    public void UpdateView(SoGoal.ProgressData progress)
    {
        m_ProgressData = progress;
        ViewInit();
    }

    void ViewInit()
    {
        txtInfo.text = string.Format("{0}", m_Index + 1);

        if (m_ProgressData.isMax)
        {
            objectProgress.SetActive(true);
            objectTick.SetActive(true);
            objectGlow.SetActive(false);
            objectNotDone.SetActive(false);
            objectDone.SetActive(true);
        }
        else
        {
            objectProgress.SetActive(m_Index <= m_ProgressData.index);
            objectTick.SetActive(m_Index < m_ProgressData.index);
            objectGlow.SetActive(m_Index == m_ProgressData.index);
            objectNotDone.SetActive(!(m_Index < m_ProgressData.index));
            objectDone.SetActive(m_Index < m_ProgressData.index);
        }

        if (m_Index == 0)
        {
            objectProgress.GetComponent<Image>().enabled = false;
        }
    }

    public void View()
    {
        bool isTickDone = false;
        bool isCanTick = false;
        objectTick.SetActive(isTickDone);
        objectGlow.SetActive(isCanTick);
    }
}
